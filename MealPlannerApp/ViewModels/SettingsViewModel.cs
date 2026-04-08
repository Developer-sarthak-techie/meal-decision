using MealPlannerApp.Models;
using MealPlannerApp.Services;

namespace MealPlannerApp.ViewModels;

public class SettingsViewModel(
    IUserProfileService userProfileService,
    IJourneyService journeyService,
    IThemeService themeService) : BaseViewModel
{
    private FitnessGoal _selectedGoal = FitnessGoal.StayFit;
    private DietPreference _selectedDietPreference = DietPreference.Veg;
    private AppThemeVariant _selectedTheme = AppThemeVariant.Ocean;
    private string _statusText = string.Empty;

    public IEnumerable<FitnessGoal> Goals { get; } = Enum.GetValues<FitnessGoal>();
    public IEnumerable<DietPreference> DietPreferences { get; } = Enum.GetValues<DietPreference>();
    public IEnumerable<AppThemeVariant> Themes { get; } = Enum.GetValues<AppThemeVariant>();

    public Command LoadCommand => new(async () => await LoadSettingsAsync());
    public Command SaveGoalCommand => new(async () => await SaveGoalAndDietAsync());
    public Command ApplyThemeCommand => new(async () => await ApplyThemeAsync());
    public Command ResetJourneyCommand => new(async () => await ResetJourneyAsync());
    public Command ResetProfileCommand => new(async () => await ResetProfileAsync());

    public FitnessGoal SelectedGoal
    {
        get => _selectedGoal;
        set => SetProperty(ref _selectedGoal, value);
    }

    public DietPreference SelectedDietPreference
    {
        get => _selectedDietPreference;
        set => SetProperty(ref _selectedDietPreference, value);
    }

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    public AppThemeVariant SelectedTheme
    {
        get => _selectedTheme;
        set => SetProperty(ref _selectedTheme, value);
    }

    // What this function does:
    // Loads current goal and diet preference to pre-fill settings UI.
    // Why it is needed:
    // Users should see and edit their latest profile settings easily.
    // Input / Output:
    // Input: Persisted profile data.
    // Output: Selected values and status labels for settings page.
    public async Task LoadSettingsAsync()
    {
        var profile = await userProfileService.GetProfileAsync();
        if (profile is null)
        {
            StatusText = "No profile found. Run onboarding first.";
            return;
        }

        SelectedGoal = profile.Goal;
        SelectedDietPreference = profile.DietPreference;
        SelectedTheme = await themeService.GetCurrentThemeAsync();
        StatusText = "Profile loaded.";
    }

    // What this function does:
    // Saves updated goal and diet preference in profile.
    // Why it is needed:
    // Gives users control over recommendation direction without reinstalling app.
    // Input / Output:
    // Input: Selected goal and diet values from UI.
    // Output: Updated persisted profile and success status text.
    public async Task SaveGoalAndDietAsync()
    {
        var profile = await userProfileService.GetProfileAsync();
        if (profile is null)
        {
            StatusText = "No profile found.";
            return;
        }

        profile.Goal = SelectedGoal;
        profile.DietPreference = SelectedDietPreference;
        await userProfileService.SaveProfileAsync(profile);
        StatusText = "Settings saved successfully.";
    }

    // What this function does:
    // Applies selected customer theme palette immediately.
    // Why it is needed:
    // Gives users premium visual personalization (Ocean/Crimson/Sunset).
    // Input / Output:
    // Input: SelectedTheme picker value.
    // Output: Updated runtime app colors and persisted theme selection.
    public async Task ApplyThemeAsync()
    {
        await themeService.ApplyThemeAsync(SelectedTheme);
        StatusText = $"Theme applied: {SelectedTheme}.";
    }

    // What this function does:
    // Clears 21-day transformation completion records.
    // Why it is needed:
    // Enables users to restart the challenge from day one.
    // Input / Output:
    // Input: None.
    // Output: Journey data reset and status update.
    public async Task ResetJourneyAsync()
    {
        await journeyService.ResetJourneyAsync();
        StatusText = "21-day journey reset.";
    }

    // What this function does:
    // Clears stored profile and resets app entry flow.
    // Why it is needed:
    // Supports full account restart and test scenarios for MVP.
    // Input / Output:
    // Input: None.
    // Output: Profile removed and app navigated to onboarding page.
    public async Task ResetProfileAsync()
    {
        await userProfileService.ResetProfileAsync();
        await journeyService.ResetJourneyAsync();
        await Shell.Current.GoToAsync("//onboarding");
    }
}
