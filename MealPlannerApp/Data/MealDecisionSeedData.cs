using MealPlannerApp.Models;

namespace MealPlannerApp.Data;

public static class MealDecisionSeedData
{
    private static readonly string[] Cuisines =
    [
        "North Indian", "South Indian", "Punjabi", "Maharashtrian", "Gujarati", "Bengali", "Indo-Chinese"
    ];

    private static readonly string[] Flavors =
    [
        "Smoky", "Tangy", "Masala", "Herbed", "Coconut", "Garlic", "Citrus"
    ];

    private static readonly string[] VegBases = ["Poha Bowl", "Paneer Curry", "Rajma Rice", "Veg Stir Fry", "Dal Tadka", "Sprouts Chaat"];
    private static readonly string[] NonVegBases = ["Chicken Curry", "Egg Bhurji", "Fish Masala", "Chicken Stir Fry", "Egg Wrap", "Tuna Snack Bowl"];

    public static readonly List<MealDecisionRecipe> Recipes = GenerateCatalog(180);

    // What this function does:
    // Generates a scalable catalog inspired by the web app context and naming patterns.
    // Why it is needed:
    // Core recommendation quality depends on a broad and diverse meal inventory.
    // Input / Output:
    // Input: Number of recipes to generate.
    // Output: List of MealDecisionRecipe items.
    private static List<MealDecisionRecipe> GenerateCatalog(int count)
    {
        var rows = new List<MealDecisionRecipe>();
        for (var i = 0; i < count; i++)
        {
            rows.Add(BuildRecipe(i));
        }

        return rows;
    }

    // What this function does:
    // Creates one synthetic but realistic recipe row with meal metadata.
    // Why it is needed:
    // Encapsulates recipe generation logic and keeps the dataset deterministic.
    // Input / Output:
    // Input: Recipe index.
    // Output: One MealDecisionRecipe object.
    private static MealDecisionRecipe BuildRecipe(int index)
    {
        var slot = (WebMealSlot)(index % 4);
        var isVeg = index % 2 == 0;
        var diet = isVeg ? WebDietMode.Veg : WebDietMode.NonVeg;
        var baseDish = isVeg ? VegBases[index % VegBases.Length] : NonVegBases[index % NonVegBases.Length];
        var cuisine = Cuisines[index % Cuisines.Length];
        var flavor = Flavors[index % Flavors.Length];
        var spice = (WebSpiceLevel)(index % 3);
        var prep = 6 + (index % 12);
        var cook = slot == WebMealSlot.Snack ? 8 + (index % 10) : 14 + (index % 24);
        var cost = Math.Min(900, 90 + (isVeg ? 0 : 120) + (index % 160) * 3);
        var calories = slot switch
        {
            WebMealSlot.Breakfast => 260 + (index % 200),
            WebMealSlot.Lunch => 350 + (index % 260),
            WebMealSlot.Dinner => 320 + (index % 280),
            _ => 180 + (index % 180)
        };
        var healthScore = 70 + (index % 28);

        return new MealDecisionRecipe
        {
            Id = $"mw-{index + 1}",
            Name = $"{cuisine} {flavor} {baseDish}",
            MealSlot = slot,
            Cuisine = cuisine,
            DietMode = diet,
            SpiceLevel = spice,
            PrepTimeMins = prep,
            CookTimeMins = cook,
            PopularityScore = 66 + (index % 30),
            EstimatedCostInr = cost,
            Calories = calories,
            HealthScore = healthScore,
            OccasionTags =
            [
                WebOccasion.Everyday,
                (WebOccasion)((index % 9) + 1)
            ],
            Ingredients =
            [
                new MealDecisionIngredient { Name = isVeg ? "Paneer" : "Chicken", Quantity = "200g" },
                new MealDecisionIngredient { Name = "Onion", Quantity = "1 medium" },
                new MealDecisionIngredient { Name = "Tomato", Quantity = "1 medium" },
                new MealDecisionIngredient { Name = "Spice Mix", Quantity = "1 tbsp" }
            ],
            Steps =
            [
                "Prep ingredients and marinate with spices.",
                "Cook aromatics and combine core ingredients.",
                "Simmer till done and finish with garnish."
            ]
        };
    }
}
