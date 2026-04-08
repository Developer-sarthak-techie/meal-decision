using MealPlannerApp.Data;
using MealPlannerApp.Models;

namespace MealPlannerApp.Services;

public class MealEngineService : IMealEngineService
{
    private readonly Random _random = new();

    // What this function does:
    // Returns smart meal options for "Aaj Kya Banaye" using profile, timing, and mood.
    // Why it is needed:
    // Upgrades random suggestions into a goal-aware recommendation engine for healthier decisions.
    // Input / Output:
    // Input: UserProfile, selected MealTime, selected MealMood, and optional number of results.
    // Output: Filtered and ranked list of Recipe suggestions.
    public Task<List<Recipe>> GetMealSuggestionsAsync(UserProfile profile, MealTime mealTime, MealMood mood, int count = 3)
    {
        var candidates = AppSeedData.Recipes
            .Where(recipe => recipe.MealTime == mealTime)
            .Where(recipe => recipe.Mood == mood)
            .Where(recipe => profile.DietPreference == DietPreference.NonVeg || recipe.IsVegetarian)
            .ToList();

        var ranked = candidates
            .Select(recipe => new { Recipe = recipe, Score = CalculateGoalAlignmentScore(profile.Goal, recipe) })
            .OrderByDescending(item => item.Score)
            .ThenBy(_ => _random.Next())
            .Take(count)
            .Select(item => item.Recipe)
            .ToList();

        if (ranked.Count > 0)
        {
            return Task.FromResult(ranked);
        }

        var fallback = AppSeedData.Recipes
            .Where(recipe => profile.DietPreference == DietPreference.NonVeg || recipe.IsVegetarian)
            .OrderBy(_ => _random.Next())
            .Take(count)
            .ToList();

        return Task.FromResult(fallback);
    }

    // What this function does:
    // Builds full-day breakfast/lunch/dinner plan based on profile goal and preference.
    // Why it is needed:
    // Drives the advanced diet planner and 21-day execution workflow.
    // Input / Output:
    // Input: UserProfile with goal and diet preference.
    // Output: Three MealPlanItem entries, one for each meal time.
    public async Task<List<MealPlanItem>> GenerateDailyPlanAsync(UserProfile profile)
    {
        var breakfast = (await GetMealSuggestionsAsync(profile, MealTime.Breakfast, MealMood.Light, 1)).First();
        var lunch = (await GetMealSuggestionsAsync(profile, MealTime.Lunch, MealMood.Heavy, 1)).First();
        var dinner = (await GetMealSuggestionsAsync(profile, MealTime.Dinner, MealMood.Light, 1)).First();

        return
        [
            new MealPlanItem { TimeSlot = MealTime.Breakfast, Recipe = breakfast },
            new MealPlanItem { TimeSlot = MealTime.Lunch, Recipe = lunch },
            new MealPlanItem { TimeSlot = MealTime.Dinner, Recipe = dinner }
        ];
    }

    // What this function does:
    // Computes how well a recipe matches a selected fitness goal.
    // Why it is needed:
    // Converts generic recipe lists into personalized nutrition-aware ranking.
    // Input / Output:
    // Input: Goal type and recipe nutrition metadata.
    // Output: Integer alignment score where higher is better.
    private static int CalculateGoalAlignmentScore(FitnessGoal goal, Recipe recipe)
    {
        return goal switch
        {
            FitnessGoal.LoseWeight => (1000 - recipe.Calories) + recipe.HealthScore,
            FitnessGoal.GainWeight => recipe.Calories + recipe.ProteinGrams * 3,
            FitnessGoal.BuildMuscle => recipe.ProteinGrams * 5 + recipe.HealthScore,
            FitnessGoal.StayFit => recipe.HealthScore * 2 - Math.Abs(450 - recipe.Calories),
            FitnessGoal.AthleticBody => recipe.ProteinGrams * 4 + recipe.HealthScore - Math.Abs(500 - recipe.Calories),
            _ => recipe.HealthScore
        };
    }
}
