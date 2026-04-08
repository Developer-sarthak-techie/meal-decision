using System.Collections.ObjectModel;
using MealPlannerApp.Models;
using MealPlannerApp.Services;

namespace MealPlannerApp.ViewModels;

public class DietPlanViewModel(
    IUserProfileService userProfileService,
    IMealEngineService mealEngineService) : BaseViewModel
{
    private int _waterGlasses;
    private string _statusText = "Generate a plan to begin.";

    public ObservableCollection<MealPlanItem> DailyPlan { get; } = [];
    public Command GeneratePlanCommand => new(async () => await GeneratePlanAsync());
    public Command AddWaterCommand => new(AddWater);
    public Command ResetWaterCommand => new(ResetWater);

    public int WaterGlasses
    {
        get => _waterGlasses;
        set => SetProperty(ref _waterGlasses, value);
    }

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    // What this function does:
    // Generates breakfast/lunch/dinner diet plan aligned to user profile.
    // Why it is needed:
    // Converts recommendation logic into a full-day actionable plan.
    // Input / Output:
    // Input: Saved user profile preferences and goal.
    // Output: Populates DailyPlan collection with meal slots.
    public async Task GeneratePlanAsync()
    {
        IsBusy = true;
        DailyPlan.Clear();

        var profile = await userProfileService.GetProfileAsync();
        if (profile is null)
        {
            StatusText = "Complete onboarding first.";
            IsBusy = false;
            return;
        }

        var plan = await mealEngineService.GenerateDailyPlanAsync(profile);
        foreach (var item in plan)
        {
            DailyPlan.Add(item);
        }

        StatusText = "Your personalized full-day meal plan is ready.";
        IsBusy = false;
    }

    // What this function does:
    // Increments consumed water glass count.
    // Why it is needed:
    // Daily hydration tracking improves habit adherence in diet plans.
    // Input / Output:
    // Input: None.
    // Output: WaterGlasses increases by one.
    public void AddWater()
    {
        WaterGlasses++;
    }

    // What this function does:
    // Resets hydration counter to zero.
    // Why it is needed:
    // Allows users to start fresh each day.
    // Input / Output:
    // Input: None.
    // Output: WaterGlasses reset and status text update.
    public void ResetWater()
    {
        WaterGlasses = 0;
        StatusText = "Water tracker reset for a new day.";
    }
}
