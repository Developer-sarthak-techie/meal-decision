using MealPlannerApp.Models;
using MealPlannerApp.Services;

namespace MealPlannerApp.ViewModels;

public class DashboardViewModel(
    IUserProfileService userProfileService,
    IFitnessService fitnessService,
    IJourneyService journeyService) : BaseViewModel
{
    private HealthSummary _summary = new();

    public Command RefreshCommand => new(async () => await LoadDashboardAsync());

    public HealthSummary Summary
    {
        get => _summary;
        set => SetProperty(ref _summary, value);
    }

    // What this function does:
    // Loads dashboard KPIs including BMI, calories target, goal summary, and 21-day progress.
    // Why it is needed:
    // Users need one quick insight screen to track transformation momentum.
    // Input / Output:
    // Input: Current stored profile and journey state.
    // Output: Updated Summary object bound to dashboard UI.
    public async Task LoadDashboardAsync()
    {
        IsBusy = true;
        var profile = await userProfileService.GetProfileAsync();
        if (profile is null)
        {
            Summary = new HealthSummary
            {
                GoalSummary = "Complete onboarding to unlock your dashboard."
            };
            IsBusy = false;
            return;
        }

        var bmi = fitnessService.CalculateBmi(profile.WeightKg, profile.HeightMeters);
        var calories = fitnessService.CalculateDailyCaloriesTarget(profile);
        var summaryText = fitnessService.BuildGoalSummary(profile, bmi);
        var streak = await journeyService.GetCurrentStreakAsync();
        var progress = await journeyService.GetProgressAsync();

        Summary = new HealthSummary
        {
            Bmi = bmi,
            DailyCaloriesTarget = calories,
            GoalSummary = summaryText,
            CurrentStreak = streak,
            JourneyProgress = progress
        };

        IsBusy = false;
    }
}
