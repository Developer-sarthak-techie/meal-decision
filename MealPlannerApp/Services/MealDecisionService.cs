using MealPlannerApp.Data;
using MealPlannerApp.Models;

namespace MealPlannerApp.Services;

public class MealDecisionService(IStorageService storageService) : IMealDecisionService
{
    private const string PreferencesKey = "meal_web_preferences";
    private const string HistoryKey = "meal_web_history";

    // What this function does:
    // Loads persisted web-style meal decision preferences.
    // Why it is needed:
    // Keeps app behavior aligned with web context and user choices across sessions.
    // Input / Output:
    // Input: None.
    // Output: Stored preferences or default preferences.
    public async Task<MealDecisionPreferences> GetPreferencesAsync()
    {
        return await storageService.GetAsync<MealDecisionPreferences>(PreferencesKey) ?? new MealDecisionPreferences();
    }

    // What this function does:
    // Persists meal decision preferences.
    // Why it is needed:
    // Ensures user settings like pantry mode, budget, and spice are remembered.
    // Input / Output:
    // Input: MealDecisionPreferences object.
    // Output: Async completion task.
    public async Task SavePreferencesAsync(MealDecisionPreferences preferences)
    {
        await storageService.SaveAsync(PreferencesKey, preferences);
    }

    // What this function does:
    // Retrieves recent recipe history used for freshness scoring.
    // Why it is needed:
    // Avoids repetitive suggestions and mirrors web app context behavior.
    // Input / Output:
    // Input: None.
    // Output: Recent recipe id list.
    public async Task<List<string>> GetHistoryAsync()
    {
        return await storageService.GetAsync<List<string>>(HistoryKey) ?? [];
    }

    // What this function does:
    // Saves recipe history list.
    // Why it is needed:
    // Preserves recommendation diversity state between app launches.
    // Input / Output:
    // Input: Recipe id list.
    // Output: Async completion task.
    public async Task SaveHistoryAsync(List<string> history)
    {
        await storageService.SaveAsync(HistoryKey, history.TakeLast(8).ToList());
    }

    // What this function does:
    // Computes one ranked suggestion result with alternatives and explanation.
    // Why it is needed:
    // Recreates the web app's core recommendation context in mobile-first form.
    // Input / Output:
    // Input: Slot/day, preferences, and recommendation history.
    // Output: Suggestion result or null when no viable meals exist.
    public Task<MealDecisionSuggestionResult?> SuggestAsync(WebMealSlot mealSlot, WebWeekday day, MealDecisionPreferences preferences, List<string> history)
    {
        var layers = new[]
        {
            new { SkipPantry = false, SkipBudget = false, SkipTime = false, Note = "" },
            new { SkipPantry = false, SkipBudget = false, SkipTime = true, Note = " Cook time relaxed." },
            new { SkipPantry = true, SkipBudget = false, SkipTime = false, Note = " Pantry filter relaxed." },
            new { SkipPantry = false, SkipBudget = true, SkipTime = false, Note = " Budget filter relaxed." },
            new { SkipPantry = true, SkipBudget = true, SkipTime = true, Note = " Multiple filters relaxed." }
        };

        List<MealDecisionRecipe> ranked = [];
        var suffix = string.Empty;
        foreach (var layer in layers)
        {
            var candidates = MealDecisionSeedData.Recipes
                .Where(r => BaseFilter(r, mealSlot, preferences, layer.SkipPantry, layer.SkipBudget, layer.SkipTime))
                .OrderByDescending(r => ScoreRecipe(r, preferences, history))
                .ToList();

            if (candidates.Count > 0)
            {
                ranked = candidates;
                suffix = layer.Note;
                break;
            }
        }

        if (ranked.Count == 0)
        {
            return Task.FromResult<MealDecisionSuggestionResult?>(null);
        }

        var primary = ranked[0];
        var alternatives = ranked.Skip(1).Take(2).ToList();
        var explanation = BuildExplanation(primary, preferences) + suffix;
        var uniqueCookIdea = BuildUniqueIdea(primary, preferences);

        return Task.FromResult<MealDecisionSuggestionResult?>(new MealDecisionSuggestionResult
        {
            PrimarySuggestion = primary,
            Alternatives = alternatives,
            Explanation = explanation,
            UniqueCookIdea = uniqueCookIdea
        });
    }

    // What this function does:
    // Builds 7-day plan across breakfast, lunch, dinner, and snack.
    // Why it is needed:
    // Matches web app's weekly routine generation as a main product capability.
    // Input / Output:
    // Input: Meal decision preferences.
    // Output: List of weekly plan rows.
    public async Task<List<MealDecisionWeeklyPlanDay>> GenerateWeeklyPlanAsync(MealDecisionPreferences preferences)
    {
        var history = await GetHistoryAsync();
        var days = Enum.GetValues<WebWeekday>();
        var rows = new List<MealDecisionWeeklyPlanDay>();

        foreach (var day in days)
        {
            var breakfast = await SuggestAsync(WebMealSlot.Breakfast, day, preferences, history);
            var lunch = await SuggestAsync(WebMealSlot.Lunch, day, preferences, history);
            var dinner = await SuggestAsync(WebMealSlot.Dinner, day, preferences, history);
            var snack = await SuggestAsync(WebMealSlot.Snack, day, preferences, history);

            if (breakfast?.PrimarySuggestion is not null) history.Add(breakfast.PrimarySuggestion.Id);
            if (lunch?.PrimarySuggestion is not null) history.Add(lunch.PrimarySuggestion.Id);
            if (dinner?.PrimarySuggestion is not null) history.Add(dinner.PrimarySuggestion.Id);
            if (snack?.PrimarySuggestion is not null) history.Add(snack.PrimarySuggestion.Id);

            rows.Add(new MealDecisionWeeklyPlanDay
            {
                Day = day,
                Breakfast = breakfast?.PrimarySuggestion,
                Lunch = lunch?.PrimarySuggestion,
                Dinner = dinner?.PrimarySuggestion,
                Snack = snack?.PrimarySuggestion
            });
        }

        await SaveHistoryAsync(history);
        return rows;
    }

    // What this function does:
    // Evaluates if recipe qualifies for current user filters.
    // Why it is needed:
    // Keeps recommendation logic explainable and consistent with web app context.
    // Input / Output:
    // Input: Recipe, slot, preferences, and optional relax flags.
    // Output: True when recipe passes active filters.
    private static bool BaseFilter(MealDecisionRecipe recipe, WebMealSlot slot, MealDecisionPreferences prefs, bool skipPantry, bool skipBudget, bool skipTime)
    {
        if (recipe.MealSlot != slot) return false;
        if (prefs.DietMode != WebDietMode.Mix && recipe.DietMode != prefs.DietMode) return false;
        if (prefs.PreferredCuisines.Count > 0 &&
            !prefs.PreferredCuisines.Any(c => c.Equals(recipe.Cuisine, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        if (prefs.ExcludedIngredients.Count > 0 &&
            recipe.Ingredients.Any(i => prefs.ExcludedIngredients.Contains(i.Name, StringComparer.OrdinalIgnoreCase)))
        {
            return false;
        }

        if (!skipPantry && prefs.PantryMode && prefs.PantryIngredients.Count > 0)
        {
            var pantry = prefs.PantryIngredients.Select(s => s.Trim().ToLowerInvariant()).ToList();
            var allCovered = recipe.Ingredients.All(ing =>
                pantry.Any(p => ing.Name.ToLowerInvariant().Contains(p) || p.Contains(ing.Name.ToLowerInvariant())));
            if (!allCovered) return false;
        }

        if (prefs.Occasion != WebOccasion.Everyday && !recipe.OccasionTags.Contains(prefs.Occasion)) return false;
        if (!skipBudget && prefs.BudgetMode && recipe.EstimatedCostInr > MaxCostInr(prefs.BudgetTier)) return false;

        var maxTime = prefs.TonightMode ? 15 : prefs.MaxCookTimeMins;
        if (!skipTime && recipe.TotalTimeMins > maxTime) return false;

        return true;
    }

    // What this function does:
    // Scores recipe relevance against preferences and recent history.
    // Why it is needed:
    // Higher scores create better ranking quality for primary and alternate picks.
    // Input / Output:
    // Input: Recipe, preferences, and previous suggestion ids.
    // Output: Numeric score.
    private static double ScoreRecipe(MealDecisionRecipe recipe, MealDecisionPreferences preferences, List<string> history)
    {
        var desiredTime = preferences.TonightMode ? 15 : preferences.MaxCookTimeMins;
        var timeFit = Math.Max(0, 30 - Math.Abs(desiredTime - recipe.TotalTimeMins));
        var spiceFit = recipe.SpiceLevel == preferences.SpiceLevel ? 20 : 8;
        var freshness = history.Contains(recipe.Id) ? -25 : 12;
        var novelty = preferences.NoveltyLevel switch
        {
            WebNoveltyLevel.Classic => recipe.PopularityScore > 80 ? 14 : 4,
            WebNoveltyLevel.Experimental => recipe.PopularityScore < 85 ? 16 : 3,
            _ => 8
        };

        var kidsBoost = preferences.KidsFriendly
            ? recipe.SpiceLevel == WebSpiceLevel.Low ? 16 : recipe.SpiceLevel == WebSpiceLevel.Medium ? 8 : -10
            : 0;

        var budgetBoost = preferences.BudgetMode
            ? Math.Max(0, (MaxCostInr(preferences.BudgetTier) - recipe.EstimatedCostInr) / 10.0)
            : 0;

        return (recipe.PopularityScore / 2.0) + timeFit + spiceFit + freshness + novelty + kidsBoost + budgetBoost + recipe.HealthScore / 8.0;
    }

    // What this function does:
    // Builds user-facing rationale text for selected recommendation.
    // Why it is needed:
    // Explanation is central to trust and mirrors web app’s context messaging.
    // Input / Output:
    // Input: Selected recipe and active preferences.
    // Output: Human-readable explanation sentence.
    private static string BuildExplanation(MealDecisionRecipe recipe, MealDecisionPreferences preferences)
    {
        var parts = new List<string>
        {
            "Matched to diet, spice preference, and recent history."
        };

        if (preferences.PantryMode && preferences.PantryIngredients.Count > 0) parts.Add("Pantry mode applied.");
        if (preferences.Occasion != WebOccasion.Everyday) parts.Add($"Occasion focus: {preferences.Occasion}.");
        if (preferences.BudgetMode) parts.Add($"Budget fit: ~INR {recipe.EstimatedCostInr}.");
        if (preferences.TonightMode) parts.Add("Tonight mode enabled for quick cooking.");
        if (preferences.KidsFriendly) parts.Add("Kids-friendly spice bias applied.");

        return string.Join(" ", parts);
    }

    // What this function does:
    // Creates a small "unique cook idea" tip for selected recipe.
    // Why it is needed:
    // Preserves signature web feature that adds novelty and delight.
    // Input / Output:
    // Input: Selected recipe and novelty preference.
    // Output: One-line unique idea tip.
    private static string BuildUniqueIdea(MealDecisionRecipe recipe, MealDecisionPreferences preferences)
    {
        var ingredient = recipe.Ingredients.FirstOrDefault()?.Name ?? "main ingredient";
        var noveltyTail = preferences.NoveltyLevel switch
        {
            WebNoveltyLevel.Classic => "Finish with simple ghee tadka for comfort flavor.",
            WebNoveltyLevel.Experimental => "Plate as lettuce wraps with chili-lime yogurt.",
            _ => "Try half classic and half with lemon zest + toasted seeds."
        };

        return $"Unique twist: dry-roast {ingredient.ToLowerInvariant()} lightly before cooking. {noveltyTail}";
    }

    // What this function does:
    // Returns max allowed estimated cost for selected budget tier.
    // Why it is needed:
    // Budget mode filtering requires deterministic cost thresholds.
    // Input / Output:
    // Input: Budget tier enum.
    // Output: INR threshold.
    private static int MaxCostInr(WebBudgetTier tier) => tier switch
    {
        WebBudgetTier.Budget => 220,
        WebBudgetTier.Moderate => 450,
        _ => 99999
    };
}
