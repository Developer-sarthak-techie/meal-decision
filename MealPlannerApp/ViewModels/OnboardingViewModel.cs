using MealPlannerApp.Models;
using MealPlannerApp.Services;

namespace MealPlannerApp.ViewModels;

public class OnboardingViewModel(IUserProfileService userProfileService) : BaseViewModel
{
    private double _heightMeters = 1.70;
    private double _weightKg = 68;
    private int _age = 25;
    private FitnessGoal _selectedGoal = FitnessGoal.StayFit;
    private DietPreference _selectedDietPreference = DietPreference.Veg;

    public IEnumerable<FitnessGoal> Goals { get; } = Enum.GetValues<FitnessGoal>();
    public IEnumerable<DietPreference> DietPreferences { get; } = Enum.GetValues<DietPreference>();

    public Command SaveProfileCommand => new(async () => await SaveProfileAsync());

    public double HeightMeters
    {
        get => _heightMeters;
        set => SetProperty(ref _heightMeters, value);
    }

    public double WeightKg
    {
        get => _weightKg;
        set => SetProperty(ref _weightKg, value);
    }

    public int Age
    {
        get => _age;
        set => SetProperty(ref _age, value);
    }

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

    // What this function does:
    // Saves profile and navigates to main meal decision page within shell.
    // Why it is needed:
    // Keeps flyout visible throughout the entire app lifecycle.
    // Input / Output:
    // Input: Form values from bound properties.
    // Output: Persisted UserProfile and shell navigation to meal decision tab.
    public async Task SaveProfileAsync()
    {
        if (HeightMeters <= 0 || WeightKg <= 0 || Age <= 0)
        {
            if (Application.Current?.Windows.FirstOrDefault()?.Page is not null)
            {
                await Application.Current.Windows[0].Page!.DisplayAlert(
                    "Invalid Input", "Please fill all values with valid numbers.", "OK");
            }
            return;
        }

        IsBusy = true;
        var profile = new UserProfile
        {
            HeightMeters = HeightMeters,
            WeightKg = WeightKg,
            Age = Age,
            Goal = SelectedGoal,
            DietPreference = SelectedDietPreference
        };

        await userProfileService.SaveProfileAsync(profile);
        await Shell.Current.GoToAsync("//aajkyabanaye");
        IsBusy = false;
    }
}
