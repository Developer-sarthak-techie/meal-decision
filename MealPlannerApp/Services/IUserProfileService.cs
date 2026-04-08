using MealPlannerApp.Models;

namespace MealPlannerApp.Services;

public interface IUserProfileService
{
    Task<UserProfile?> GetProfileAsync();
    Task SaveProfileAsync(UserProfile profile);
    Task UpdateGoalAsync(FitnessGoal goal);
    Task UpdateDietPreferenceAsync(DietPreference preference);
    Task ResetProfileAsync();
}
