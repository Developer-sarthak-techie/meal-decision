using MealPlannerApp.Models;

namespace MealPlannerApp.Services;

public interface IMealDecisionService
{
    Task<MealDecisionPreferences> GetPreferencesAsync();
    Task SavePreferencesAsync(MealDecisionPreferences preferences);
    Task<List<string>> GetHistoryAsync();
    Task SaveHistoryAsync(List<string> history);
    Task<MealDecisionSuggestionResult?> SuggestAsync(WebMealSlot mealSlot, WebWeekday day, MealDecisionPreferences preferences, List<string> history);
    Task<List<MealDecisionWeeklyPlanDay>> GenerateWeeklyPlanAsync(MealDecisionPreferences preferences);
}
