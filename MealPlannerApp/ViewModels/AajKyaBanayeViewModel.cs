using System.Collections.ObjectModel;
using MealPlannerApp.Models;
using MealPlannerApp.Services;

namespace MealPlannerApp.ViewModels;

public class AajKyaBanayeViewModel(IMealDecisionService mealDecisionService) : BaseViewModel
{
    private WebMealSlot _selectedMealSlot = WebMealSlot.Dinner;
    private WebWeekday _selectedDay = WebWeekday.Mon;
    private WebDietMode _selectedDietMode = WebDietMode.Mix;
    private WebSpiceLevel _selectedSpiceLevel = WebSpiceLevel.Medium;
    private WebNoveltyLevel _selectedNovelty = WebNoveltyLevel.Balanced;
    private WebOccasion _selectedOccasion = WebOccasion.Everyday;
    private WebBudgetTier _selectedBudgetTier = WebBudgetTier.Moderate;
    private string _pantryInput = string.Empty;
    private string _statusText = "Set your context and tap Suggest now.";
    private bool _pantryMode;
    private bool _budgetMode;
    private bool _tonightMode;
    private bool _kidsFriendly;
    private int _maxCookTime = 35;
    private MealDecisionRecipe? _primarySuggestion;
    private string _explanation = string.Empty;
    private string _uniqueCookIdea = string.Empty;

    public ObservableCollection<MealDecisionRecipe> Alternatives { get; } = [];
    public ObservableCollection<MealDecisionWeeklyPlanDay> WeeklyPlan { get; } = [];
    public IEnumerable<WebMealSlot> MealSlots { get; } = Enum.GetValues<WebMealSlot>();
    public IEnumerable<WebWeekday> Weekdays { get; } = Enum.GetValues<WebWeekday>();
    public IEnumerable<WebDietMode> DietModes { get; } = Enum.GetValues<WebDietMode>();
    public IEnumerable<WebSpiceLevel> SpiceLevels { get; } = Enum.GetValues<WebSpiceLevel>();
    public IEnumerable<WebNoveltyLevel> NoveltyLevels { get; } = Enum.GetValues<WebNoveltyLevel>();
    public IEnumerable<WebOccasion> Occasions { get; } = Enum.GetValues<WebOccasion>();
    public IEnumerable<WebBudgetTier> BudgetTiers { get; } = Enum.GetValues<WebBudgetTier>();

    public Command SuggestCommand => new(async () => await GenerateSuggestionsAsync());
    public Command SwapCommand => new(SwapSuggestion);
    public Command GenerateWeekCommand => new(async () => await GenerateWeeklyPlanAsync());
    public Command ApplyPgPresetCommand => new(ApplyPgPreset);
    public Command ApplyFamilyPresetCommand => new(ApplyFamilyPreset);
    public Command ApplyBudgetPresetCommand => new(ApplyBudgetPreset);

    public WebMealSlot SelectedMealSlot
    {
        get => _selectedMealSlot;
        set => SetProperty(ref _selectedMealSlot, value);
    }

    public WebWeekday SelectedDay
    {
        get => _selectedDay;
        set => SetProperty(ref _selectedDay, value);
    }

    public WebDietMode SelectedDietMode
    {
        get => _selectedDietMode;
        set => SetProperty(ref _selectedDietMode, value);
    }

    public WebSpiceLevel SelectedSpiceLevel
    {
        get => _selectedSpiceLevel;
        set => SetProperty(ref _selectedSpiceLevel, value);
    }

    public WebNoveltyLevel SelectedNovelty
    {
        get => _selectedNovelty;
        set => SetProperty(ref _selectedNovelty, value);
    }

    public WebOccasion SelectedOccasion
    {
        get => _selectedOccasion;
        set => SetProperty(ref _selectedOccasion, value);
    }

    public WebBudgetTier SelectedBudgetTier
    {
        get => _selectedBudgetTier;
        set => SetProperty(ref _selectedBudgetTier, value);
    }

    public string PantryInput
    {
        get => _pantryInput;
        set => SetProperty(ref _pantryInput, value);
    }

    public bool PantryMode
    {
        get => _pantryMode;
        set => SetProperty(ref _pantryMode, value);
    }

    public bool BudgetMode
    {
        get => _budgetMode;
        set => SetProperty(ref _budgetMode, value);
    }

    public bool TonightMode
    {
        get => _tonightMode;
        set => SetProperty(ref _tonightMode, value);
    }

    public bool KidsFriendly
    {
        get => _kidsFriendly;
        set => SetProperty(ref _kidsFriendly, value);
    }

    public int MaxCookTime
    {
        get => _maxCookTime;
        set => SetProperty(ref _maxCookTime, value);
    }

    public MealDecisionRecipe? PrimarySuggestion
    {
        get => _primarySuggestion;
        set => SetProperty(ref _primarySuggestion, value);
    }

    public string Explanation
    {
        get => _explanation;
        set => SetProperty(ref _explanation, value);
    }

    public string UniqueCookIdea
    {
        get => _uniqueCookIdea;
        set => SetProperty(ref _uniqueCookIdea, value);
    }

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    // What this function does:
    // Loads saved preferences from local storage into web-style control fields.
    // Why it is needed:
    // Keeps the mobile context aligned with existing web app behavior and continuity.
    // Input / Output:
    // Input: Stored preference object from service.
    // Output: Updates local bindable state.
    public async Task InitializeAsync()
    {
        var pref = await mealDecisionService.GetPreferencesAsync();
        SelectedDietMode = pref.DietMode;
        SelectedSpiceLevel = pref.SpiceLevel;
        SelectedNovelty = pref.NoveltyLevel;
        SelectedOccasion = pref.Occasion;
        SelectedBudgetTier = pref.BudgetTier;
        PantryMode = pref.PantryMode;
        BudgetMode = pref.BudgetMode;
        TonightMode = pref.TonightMode;
        KidsFriendly = pref.KidsFriendly;
        MaxCookTime = pref.MaxCookTimeMins;
        PantryInput = string.Join(", ", pref.PantryIngredients);
    }

    // What this function does:
    // Generates primary recommendation + alternatives using web-context decision logic.
    // Why it is needed:
    // Restores the original product focus where meal decisioning is the main experience.
    // Input / Output:
    // Input: Current control values and stored recommendation history.
    // Output: Updates primary card, alternatives, explanation, and status.
    public async Task GenerateSuggestionsAsync()
    {
        IsBusy = true;
        Alternatives.Clear();

        var pref = BuildPreferencesFromState();
        await mealDecisionService.SavePreferencesAsync(pref);
        var history = await mealDecisionService.GetHistoryAsync();
        var result = await mealDecisionService.SuggestAsync(SelectedMealSlot, SelectedDay, pref, history);

        if (result is null)
        {
            PrimarySuggestion = null;
            Explanation = string.Empty;
            UniqueCookIdea = string.Empty;
            StatusText = "No matching recipe found for current filters.";
            IsBusy = false;
            return;
        }

        PrimarySuggestion = result.PrimarySuggestion;
        Explanation = result.Explanation;
        UniqueCookIdea = result.UniqueCookIdea;
        foreach (var alt in result.Alternatives)
        {
            Alternatives.Add(alt);
        }

        history.Add(result.PrimarySuggestion.Id);
        await mealDecisionService.SaveHistoryAsync(history);
        StatusText = "Suggestion ready.";
        IsBusy = false;
    }

    // What this function does:
    // Swaps to the first alternative and rotates current primary into alternatives.
    // Why it is needed:
    // Mimics web app "Swap suggestion" action without recomputing from scratch.
    // Input / Output:
    // Input: Existing recommendation state.
    // Output: New primary suggestion and reordered alternatives.
    public void SwapSuggestion()
    {
        if (PrimarySuggestion is null || Alternatives.Count == 0)
        {
            return;
        }

        var next = Alternatives[0];
        Alternatives.RemoveAt(0);
        Alternatives.Add(PrimarySuggestion);
        PrimarySuggestion = next;
        StatusText = "Swapped to alternate suggestion.";
    }

    // What this function does:
    // Generates a week plan with breakfast/lunch/dinner/snack entries.
    // Why it is needed:
    // Weekly routine planning is a core web app capability requested for mobile.
    // Input / Output:
    // Input: Active web-style preferences.
    // Output: Populates WeeklyPlan collection.
    public async Task GenerateWeeklyPlanAsync()
    {
        IsBusy = true;
        WeeklyPlan.Clear();
        var pref = BuildPreferencesFromState();
        await mealDecisionService.SavePreferencesAsync(pref);
        var rows = await mealDecisionService.GenerateWeeklyPlanAsync(pref);
        foreach (var row in rows)
        {
            WeeklyPlan.Add(row);
        }

        StatusText = "Weekly plan generated.";
        IsBusy = false;
    }

    // What this function does:
    // Applies PG/Hostel quick preset similar to web app.
    // Why it is needed:
    // Helps users configure constraints quickly for practical daily use.
    // Input / Output:
    // Input: None.
    // Output: Updates settings state fields.
    public void ApplyPgPreset()
    {
        SelectedDietMode = WebDietMode.Veg;
        MaxCookTime = 30;
        BudgetMode = true;
        SelectedBudgetTier = WebBudgetTier.Budget;
        SelectedNovelty = WebNoveltyLevel.Classic;
        TonightMode = false;
        StatusText = "Applied PG / Hostel preset.";
    }

    // What this function does:
    // Applies family vegetarian week preset.
    // Why it is needed:
    // Mirrors the web app's one-tap family setup behavior.
    // Input / Output:
    // Input: None.
    // Output: Updates settings state fields.
    public void ApplyFamilyPreset()
    {
        SelectedDietMode = WebDietMode.Veg;
        MaxCookTime = 45;
        BudgetMode = false;
        SelectedBudgetTier = WebBudgetTier.Moderate;
        SelectedNovelty = WebNoveltyLevel.Balanced;
        TonightMode = false;
        StatusText = "Applied Family Veg Week preset.";
    }

    // What this function does:
    // Applies budget-week preset.
    // Why it is needed:
    // Gives fast access to cost-optimized recommendation mode.
    // Input / Output:
    // Input: None.
    // Output: Updates settings state fields.
    public void ApplyBudgetPreset()
    {
        SelectedDietMode = WebDietMode.Mix;
        MaxCookTime = 40;
        BudgetMode = true;
        SelectedBudgetTier = WebBudgetTier.Budget;
        SelectedNovelty = WebNoveltyLevel.Classic;
        TonightMode = false;
        StatusText = "Applied Budget Week preset.";
    }

    // What this function does:
    // Maps bindable UI state into one preference object.
    // Why it is needed:
    // Keeps conversion logic centralized and avoids inconsistent persistence.
    // Input / Output:
    // Input: Current ViewModel fields.
    // Output: MealDecisionPreferences object.
    private MealDecisionPreferences BuildPreferencesFromState()
    {
        var pantry = PantryInput
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        return new MealDecisionPreferences
        {
            DietMode = SelectedDietMode,
            SpiceLevel = SelectedSpiceLevel,
            MaxCookTimeMins = MaxCookTime,
            NoveltyLevel = SelectedNovelty,
            PantryMode = PantryMode,
            PantryIngredients = pantry,
            Occasion = SelectedOccasion,
            BudgetMode = BudgetMode,
            BudgetTier = SelectedBudgetTier,
            TonightMode = TonightMode,
            KidsFriendly = KidsFriendly
        };
    }
}
