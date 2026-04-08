using MealPlannerApp.Models;

namespace MealPlannerApp.Services;

public interface IJourneyService
{
    Task<List<JourneyTask>> GetTasksAsync();
    Task ToggleTaskCompletionAsync(string taskId);
    Task ResetJourneyAsync();
    Task<int> GetCurrentStreakAsync();
    Task<double> GetProgressAsync();
}
