namespace MealPlannerApp.Models;

public enum WebDietMode
{
    Veg,
    NonVeg,
    Mix
}

public enum WebMealSlot
{
    Breakfast,
    Lunch,
    Dinner,
    Snack
}

public enum WebSpiceLevel
{
    Low,
    Medium,
    High
}

public enum WebWeekday
{
    Mon,
    Tue,
    Wed,
    Thu,
    Fri,
    Sat,
    Sun
}

public enum WebNoveltyLevel
{
    Classic,
    Balanced,
    Experimental
}

public enum WebOccasion
{
    Everyday,
    Diwali,
    Holi,
    Eid,
    Christmas,
    Navratri,
    GaneshChaturthi,
    Onam,
    Birthday,
    Party
}

public enum WebBudgetTier
{
    Budget,
    Moderate,
    Premium
}

public class MealDecisionPreferences
{
    public WebDietMode DietMode { get; set; } = WebDietMode.Mix;
    public WebSpiceLevel SpiceLevel { get; set; } = WebSpiceLevel.Medium;
    public int MaxCookTimeMins { get; set; } = 35;
    public List<string> ExcludedIngredients { get; set; } = [];
    public List<string> PreferredCuisines { get; set; } = [];
    public WebNoveltyLevel NoveltyLevel { get; set; } = WebNoveltyLevel.Balanced;
    public bool PantryMode { get; set; }
    public List<string> PantryIngredients { get; set; } = [];
    public WebOccasion Occasion { get; set; } = WebOccasion.Everyday;
    public bool BudgetMode { get; set; }
    public WebBudgetTier BudgetTier { get; set; } = WebBudgetTier.Moderate;
    public bool TonightMode { get; set; }
    public bool KidsFriendly { get; set; }
}

public class MealDecisionIngredient
{
    public string Name { get; set; } = string.Empty;
    public string Quantity { get; set; } = string.Empty;
}

public class MealDecisionRecipe
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public WebMealSlot MealSlot { get; set; }
    public string Cuisine { get; set; } = string.Empty;
    public WebDietMode DietMode { get; set; }
    public WebSpiceLevel SpiceLevel { get; set; }
    public int PrepTimeMins { get; set; }
    public int CookTimeMins { get; set; }
    public int PopularityScore { get; set; }
    public int EstimatedCostInr { get; set; }
    public int Calories { get; set; }
    public int HealthScore { get; set; }
    public List<WebOccasion> OccasionTags { get; set; } = [];
    public List<MealDecisionIngredient> Ingredients { get; set; } = [];
    public List<string> Steps { get; set; } = [];
    public int TotalTimeMins => PrepTimeMins + CookTimeMins;
}

public class MealDecisionSuggestionResult
{
    public MealDecisionRecipe PrimarySuggestion { get; set; } = new();
    public List<MealDecisionRecipe> Alternatives { get; set; } = [];
    public string Explanation { get; set; } = string.Empty;
    public string UniqueCookIdea { get; set; } = string.Empty;
}

public class MealDecisionWeeklyPlanDay
{
    public WebWeekday Day { get; set; }
    public MealDecisionRecipe? Breakfast { get; set; }
    public MealDecisionRecipe? Lunch { get; set; }
    public MealDecisionRecipe? Dinner { get; set; }
    public MealDecisionRecipe? Snack { get; set; }
}
