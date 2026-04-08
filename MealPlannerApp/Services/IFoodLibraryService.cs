using MealPlannerApp.Models;

namespace MealPlannerApp.Services;

public interface IFoodLibraryService
{
    Task<List<FoodLibraryItem>> GetFoodsAsync();
}
