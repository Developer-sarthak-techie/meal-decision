using MealPlannerApp.Data;
using MealPlannerApp.Models;

namespace MealPlannerApp.Services;

public class FoodLibraryService : IFoodLibraryService
{
    // What this function does:
    // Returns curated natural food cards for the visual food library.
    // Why it is needed:
    // Provides quick education around food benefits and best intake timing.
    // Input / Output:
    // Input: None.
    // Output: Full list of FoodLibraryItem entries.
    public Task<List<FoodLibraryItem>> GetFoodsAsync()
    {
        return Task.FromResult(AppSeedData.Foods);
    }
}
