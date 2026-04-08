using MealPlannerApp.Models;

namespace MealPlannerApp.Services;

public class UserProfileService(IStorageService storageService) : IUserProfileService
{
    private const string ProfileKey = "user_profile";

    // What this function does:
    // Loads user profile from local storage.
    // Why it is needed:
    // Profile data drives recommendations, dashboard metrics, and onboarding state.
    // Input / Output:
    // Input: None.
    // Output: Stored UserProfile or null if not created.
    public async Task<UserProfile?> GetProfileAsync()
    {
        return await storageService.GetAsync<UserProfile>(ProfileKey);
    }

    // What this function does:
    // Persists a full user profile object.
    // Why it is needed:
    // Central write API keeps profile updates consistent across pages.
    // Input / Output:
    // Input: UserProfile object.
    // Output: Async completion task once save is done.
    public async Task SaveProfileAsync(UserProfile profile)
    {
        await storageService.SaveAsync(ProfileKey, profile);
    }

    // What this function does:
    // Updates only the fitness goal in existing profile.
    // Why it is needed:
    // Settings screen should allow goal changes without re-running onboarding.
    // Input / Output:
    // Input: New fitness goal.
    // Output: Async completion task after save.
    public async Task UpdateGoalAsync(FitnessGoal goal)
    {
        var profile = await GetProfileAsync() ?? new UserProfile();
        profile.Goal = goal;
        await SaveProfileAsync(profile);
    }

    // What this function does:
    // Updates user's veg/non-veg preference in existing profile.
    // Why it is needed:
    // Meal engine depends on diet preference for safe and relevant suggestions.
    // Input / Output:
    // Input: DietPreference enum value.
    // Output: Async completion task after save.
    public async Task UpdateDietPreferenceAsync(DietPreference preference)
    {
        var profile = await GetProfileAsync() ?? new UserProfile();
        profile.DietPreference = preference;
        await SaveProfileAsync(profile);
    }

    // What this function does:
    // Clears all stored profile data.
    // Why it is needed:
    // Enables full user reset for testing and fresh onboarding.
    // Input / Output:
    // Input: None.
    // Output: Async completion task once removed.
    public async Task ResetProfileAsync()
    {
        await storageService.RemoveAsync(ProfileKey);
    }
}
