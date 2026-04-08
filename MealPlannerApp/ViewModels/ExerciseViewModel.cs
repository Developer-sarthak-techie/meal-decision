using System.Collections.ObjectModel;
using MealPlannerApp.Models;
using MealPlannerApp.Services;

namespace MealPlannerApp.ViewModels;

public class ExerciseViewModel(
    IUserProfileService userProfileService,
    IFitnessService fitnessService) : BaseViewModel
{
    private string _statusText = "Loading exercises...";

    public ObservableCollection<ExerciseItem> Exercises { get; } = [];
    public Command LoadCommand => new(async () => await LoadExercisesAsync());

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    // What this function does:
    // Loads exercise list tailored to user's selected fitness goal.
    // Why it is needed:
    // Ensures workout recommendations remain personalized and actionable.
    // Input / Output:
    // Input: Stored user profile goal.
    // Output: Populates Exercises collection and status text.
    public async Task LoadExercisesAsync()
    {
        IsBusy = true;
        Exercises.Clear();

        var profile = await userProfileService.GetProfileAsync();
        if (profile is null)
        {
            StatusText = "Complete onboarding to unlock exercise guidance.";
            IsBusy = false;
            return;
        }

        var items = await fitnessService.GetExercisesForGoalAsync(profile.Goal);
        foreach (var exercise in items)
        {
            Exercises.Add(exercise);
        }

        StatusText = items.Count == 0 ? "No exercises found for this goal." : string.Empty;
        IsBusy = false;
    }
}
