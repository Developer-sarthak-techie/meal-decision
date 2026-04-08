namespace MealPlannerApp.Models;

public class Recipe
{
    public string DishName { get; set; } = string.Empty;
    public bool IsVegetarian { get; set; }
    public MealTime MealTime { get; set; }
    public MealMood Mood { get; set; }
    public int Calories { get; set; }
    public int ProteinGrams { get; set; }
    public int HealthScore { get; set; }
    public List<string> Ingredients { get; set; } = [];
    public string QuickRecipe { get; set; } = string.Empty;
}
