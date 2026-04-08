using MealPlannerApp.Data;
using MealPlannerApp.Models;

namespace MealPlannerApp.Services;

public class JourneyService(IStorageService storageService) : IJourneyService
{
    private const string CompletedTaskIdsKey = "journey_completed_tasks";

    // What this function does:
    // Builds all 21-day tasks with completion status.
    // Why it is needed:
    // Journey screen needs one unified source for meal and exercise task states.
    // Input / Output:
    // Input: None.
    // Output: Full task list with IsCompleted flags.
    public async Task<List<JourneyTask>> GetTasksAsync()
    {
        var completed = await GetCompletedTaskIdsAsync();
        return AppSeedData.BaseJourneyTasks
            .Select(task => new JourneyTask
            {
                Day = task.Day,
                Type = task.Type,
                Title = task.Title,
                IsCompleted = completed.Contains(task.TaskId)
            })
            .ToList();
    }

    // What this function does:
    // Toggles completion state for a given journey task.
    // Why it is needed:
    // Enables users to mark daily meal/exercise wins and maintain accountability.
    // Input / Output:
    // Input: Unique task identifier.
    // Output: Async completion task after persistence.
    public async Task ToggleTaskCompletionAsync(string taskId)
    {
        var completed = await GetCompletedTaskIdsAsync();
        if (!completed.Add(taskId))
        {
            completed.Remove(taskId);
        }

        await storageService.SaveAsync(CompletedTaskIdsKey, completed.ToList());
    }

    // What this function does:
    // Clears all completion records for a fresh 21-day restart.
    // Why it is needed:
    // Supports reset journey action requested in settings module.
    // Input / Output:
    // Input: None.
    // Output: Async completion task after deletion.
    public async Task ResetJourneyAsync()
    {
        await storageService.RemoveAsync(CompletedTaskIdsKey);
    }

    // What this function does:
    // Calculates the current consecutive-day streak from day 1 onward.
    // Why it is needed:
    // Streak is a motivational KPI displayed on dashboard and journey page.
    // Input / Output:
    // Input: None.
    // Output: Number of fully completed consecutive days.
    public async Task<int> GetCurrentStreakAsync()
    {
        var tasks = await GetTasksAsync();
        var streak = 0;

        for (var day = 1; day <= 21; day++)
        {
            var dayTasks = tasks.Where(task => task.Day == day).ToList();
            if (dayTasks.All(task => task.IsCompleted))
            {
                streak++;
            }
            else
            {
                break;
            }
        }

        return streak;
    }

    // What this function does:
    // Computes total progress percentage for 21-day transformation tasks.
    // Why it is needed:
    // Drives progress bars on dashboard and journey module.
    // Input / Output:
    // Input: None.
    // Output: Progress value from 0.0 to 1.0.
    public async Task<double> GetProgressAsync()
    {
        var tasks = await GetTasksAsync();
        var completedCount = tasks.Count(task => task.IsCompleted);
        return tasks.Count == 0 ? 0 : (double)completedCount / tasks.Count;
    }

    // What this function does:
    // Retrieves completed task ids from storage into a hash set.
    // Why it is needed:
    // Hash set enables fast lookups and toggle updates for journey operations.
    // Input / Output:
    // Input: None.
    // Output: Set of completed journey task ids.
    private async Task<HashSet<string>> GetCompletedTaskIdsAsync()
    {
        var completedList = await storageService.GetAsync<List<string>>(CompletedTaskIdsKey) ?? [];
        return completedList.ToHashSet();
    }
}
