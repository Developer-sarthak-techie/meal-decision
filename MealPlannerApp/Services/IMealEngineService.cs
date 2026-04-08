using MealPlannerApp.Models;

namespace MealPlannerApp.Services;

public interface IMealEngineService
{
    Task<List<Recipe>> GetMealSuggestionsAsync(UserProfile profile, MealTime mealTime, MealMood mood, int count = 3);
    Task<List<MealPlanItem>> GenerateDailyPlanAsync(UserProfile profile);
}
