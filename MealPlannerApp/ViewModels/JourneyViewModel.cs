using System.Collections.ObjectModel;
using MealPlannerApp.Models;
using MealPlannerApp.Services;

namespace MealPlannerApp.ViewModels;

public class JourneyViewModel(IJourneyService journeyService) : BaseViewModel
{
    private double _progress;
    private int _streak;

    public ObservableCollection<JourneyTask> Tasks { get; } = [];
    public Command LoadCommand => new(async () => await LoadTasksAsync());
    public Command<JourneyTask> ToggleTaskCommand => new(async task => await ToggleTaskAsync(task));

    public double Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }

    public int Streak
    {
        get => _streak;
        set => SetProperty(ref _streak, value);
    }

    // What this function does:
    // Loads 21-day transformation tasks and progress metrics.
    // Why it is needed:
    // Central state fetch keeps journey UI synced with completed tasks.
    // Input / Output:
    // Input: Persisted journey store.
    // Output: Updated task collection, progress, and streak values.
    public async Task LoadTasksAsync()
    {
        IsBusy = true;
        Tasks.Clear();

        var loadedTasks = await journeyService.GetTasksAsync();
        foreach (var task in loadedTasks)
        {
            Tasks.Add(task);
        }

        Progress = await journeyService.GetProgressAsync();
        Streak = await journeyService.GetCurrentStreakAsync();
        IsBusy = false;
    }

    // What this function does:
    // Toggles completion state of an individual journey task and refreshes metrics.
    // Why it is needed:
    // Allows check/uncheck behavior with immediate progress feedback.
    // Input / Output:
    // Input: Selected JourneyTask item.
    // Output: Persisted task state and refreshed screen data.
    public async Task ToggleTaskAsync(JourneyTask? task)
    {
        if (task is null)
        {
            return;
        }

        await journeyService.ToggleTaskCompletionAsync(task.TaskId);
        await LoadTasksAsync();
    }
}
