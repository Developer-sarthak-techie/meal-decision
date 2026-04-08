using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Data;

public static partial class RecipeCatalog
{
    /// <summary>Legacy themed recipes per slot (before Indian bulk merge). Use <see cref="CountInCategory"/> for totals.</summary>
    public const int LegacyRecipesPerCategory = 52;

    private static readonly string[] Styles = ["Classic", "Street Style", "Home Style", "Express"];

    private static readonly Lazy<IReadOnlyDictionary<RecipeCategoryKind, IReadOnlyList<Recipe>>> Cache = new(BuildAll);

    public static IReadOnlyDictionary<RecipeCategoryKind, IReadOnlyList<Recipe>> ByCategory => Cache.Value;

    public static int CountInCategory(RecipeCategoryKind category) => ByCategory[category].Count;

    public static Recipe? FindById(string id)
    {
        foreach (var list in ByCategory.Values)
        {
            foreach (var r in list)
            {
                if (r.Id == id)
                    return r;
            }
        }
        return null;
    }

    /// <summary>Picks a random recipe matching diet + meal slot. Returns null if none (should not happen with Mix).</summary>
    public static Recipe? SuggestRecipe(MealDietPreference preference, RecipeCategoryKind mealFor)
    {
        IEnumerable<Recipe> q = ByCategory[mealFor];
        q = preference switch
        {
            MealDietPreference.Veg => q.Where(r => r.DietType == MealDietType.Vegetarian),
            MealDietPreference.NonVeg => q.Where(r => r.DietType == MealDietType.NonVegetarian),
            _ => q
        };
        var list = q.ToList();
        if (list.Count == 0)
            return null;
        return list[Random.Shared.Next(list.Count)];
    }

    public static IReadOnlyList<Recipe> GetRecipes(RecipeCategoryKind mealFor, MealDietPreference preference)
    {
        IEnumerable<Recipe> q = ByCategory[mealFor];
        q = preference switch
        {
            MealDietPreference.Veg => q.Where(r => r.DietType == MealDietType.Vegetarian),
            MealDietPreference.NonVeg => q.Where(r => r.DietType == MealDietType.NonVegetarian),
            _ => q
        };
        var list = q.ToList();
        return list.Count == 0 ? ByCategory[mealFor] : list;
    }

    /// <summary>Extra hand-holding for first-time cooks — prepended + appended to core steps.</summary>
    public static IReadOnlyList<string> GetNoobFriendlySteps(Recipe recipe)
    {
        var head = new[]
        {
            $"Read all steps once before turning on the stove. Budget about {recipe.TotalTimeMinutes} minutes from start to eating (includes resting).",
            "Wash hands, clear counter space, grab a tasting spoon, a plate to hold chopped items, and a kitchen towel.",
            "If you use gas: keep a lid nearby to smother flare-ups; if induction: medium heat is usually your friend.",
            "Taste salt only after spices have cooked at least a minute — salt feels stronger on raw onion than in a finished gravy."
        };
        var tail = new[]
        {
            "Turn off heat, wipe the rim of the bowl or plate so it looks neat, and serve while it still looks exciting.",
            "After eating, soak pans with warm water — cleanup is easier than it looks."
        };
        return head.Concat(recipe.Steps).Concat(tail).ToArray();
    }

    private static IReadOnlyDictionary<RecipeCategoryKind, IReadOnlyList<Recipe>> BuildAll()
    {
        return new Dictionary<RecipeCategoryKind, IReadOnlyList<Recipe>>
        {
            [RecipeCategoryKind.Breakfast] = MergeRecipes(
                BuildCategory(RecipeCategoryKind.Breakfast, BreakfastBases),
                BuildIndianBulk(RecipeCategoryKind.Breakfast)),
            [RecipeCategoryKind.Lunch] = MergeRecipes(
                BuildCategory(RecipeCategoryKind.Lunch, LunchBases),
                BuildIndianBulk(RecipeCategoryKind.Lunch)),
            [RecipeCategoryKind.Dinner] = MergeRecipes(
                BuildCategory(RecipeCategoryKind.Dinner, DinnerBases),
                BuildIndianBulk(RecipeCategoryKind.Dinner)),
            [RecipeCategoryKind.Snacks] = MergeRecipes(
                BuildCategory(RecipeCategoryKind.Snacks, SnacksBases),
                BuildIndianBulk(RecipeCategoryKind.Snacks))
        };
    }

    private static IReadOnlyList<Recipe> MergeRecipes(IReadOnlyList<Recipe> legacy, IReadOnlyList<Recipe> indian)
    {
        var merged = new List<Recipe>(legacy.Count + indian.Count);
        merged.AddRange(legacy);
        merged.AddRange(indian);
        return merged;
    }

    private static IReadOnlyList<Recipe> BuildCategory(RecipeCategoryKind category, string[] bases)
    {
        var list = new List<Recipe>(LegacyRecipesPerCategory);
        var n = 0;
        foreach (var style in Styles)
        {
            foreach (var b in bases)
            {
                var title = $"{b} — {style}";
                var id = $"recipe-{category}-{n:D3}";
                var diet = InferDiet(b);
                var prep = 12 + n % 35;
                var passive = 8 + n % 30;
                list.Add(new Recipe
                {
                    Id = id,
                    Title = title,
                    Category = category,
                    ImageKey = $"recipe_dish_{(n % 16) + 1:D2}",
                    Ingredients = IngredientsFor(category, b),
                    Steps = MakeSteps(category, title, n, diet),
                    PrepMinutes = prep,
                    TotalTimeMinutes = prep + passive,
                    DietType = diet
                });
                n++;
            }
        }
        return list;
    }

    private static MealDietType InferDiet(string baseName)
    {
        var c = StringComparison.OrdinalIgnoreCase;
        if (baseName.Contains("Chicken", c) || baseName.Contains("Murgh", c) || baseName.Contains("Fish", c)
            || baseName.Contains("Mutton", c) || baseName.Contains("Gosht", c) || baseName.Contains("Prawn", c)
            || baseName.Contains("Keema", c) || baseName.Contains("Sausage", c) || baseName.Contains("Lamb", c)
            || baseName.Contains("Seekh", c) || baseName.Contains("Beef", c) || baseName.Contains("Pork", c)
            || baseName.Contains("Karimeen", c) || baseName.Contains("Maach", c) || baseName.Contains("Mach", c)
            || baseName.Contains("Meen", c) || baseName.Contains("Bombil", c) || baseName.Contains("Nihari", c)
            || baseName.Contains("Paya", c) || baseName.Contains("Paye", c) || baseName.Contains("Kodi", c))
            return MealDietType.NonVegetarian;
        if (baseName.Contains("Egg", c) || baseName.Contains("Anda", c) || baseName.Contains("Omelette", c))
            return MealDietType.NonVegetarian;
        if (baseName.Contains("French Toast", c))
            return MealDietType.NonVegetarian;
        return MealDietType.Vegetarian;
    }

    private static readonly string[] BreakfastBases =
    [
        "Masala Oats Bowl", "Vegetable Upma", "Avalakki Poha", "Stuffed Aloo Paratha",
        "Idli with Sambar", "Moong Dal Chilla", "Besan Cheela", "Ragi Dosa Stack",
        "Fruit Yogurt Parfait", "Masala Egg Bhurji", "Paneer Veg Sandwich", "Cinnamon French Toast",
        "Granola Honey Bowl"
    ];

    private static readonly string[] LunchBases =
    [
        "Rajma Chawal Bowl", "Yellow Dal Tadka Meal", "Lemon Rice Thali", "Chickpea Curry Plate",
        "Veg Biryani Lite", "Palak Paneer Combo", "Chole Kulcha Plate", "Mixed Veg Khichdi",
        "Caprese Pesto Pasta", "Tandoori Prawn Quinoa", "Thai Basil Chicken Rice", "Grilled Fish Wrap Bowl",
        "Teriyaki Chicken Don"
    ];

    private static readonly string[] DinnerBases =
    [
        "Dal Makhani Dinner", "Baingan Bharta Night", "Stuffed Bell Peppers", "Malai Kofta Curry",
        "Coconut Veg Stew", "Kadhai Paneer Feast", "Fish Malabari Curry Bowl", "Mushroom Stroganoff",
        "Chicken Kofta Taco Night", "Jackfruit Taco Night", "Chicken Lasagna Bake", "Soya Chunk Curry Bowl",
        "Vegetable Korma Plate"
    ];

    private static readonly string[] SnacksBases =
    [
        "Masala Murmura", "Roasted Makhana", "Sprouts Chaat Cup", "Baked Samosa Bites",
        "Hummus Veg Sticks", "Peanut Chikki Squares", "Fruit Chaat Bowl", "Dry Dhokla Squares",
        "Moong Salad Jar", "Bhel Puri Cup", "Egg Chilli Cheese Toast", "Yogurt Berry Pot",
        "Chicken Seekh Kebab Bites"
    ];

    private static IReadOnlyList<string> MakeSteps(RecipeCategoryKind cat, string title, int n, MealDietType diet)
    {
        var intro = cat switch
        {
            RecipeCategoryKind.Breakfast => "Warm a pan on medium heat and gather every ingredient for your breakfast plate.",
            RecipeCategoryKind.Lunch => "Rinse rice or grains and soak legumes if your lunch needs a softer texture.",
            RecipeCategoryKind.Dinner => "Prep a wide pan, chop aromatics, and line up spices before you start the dinner simmer.",
            _ => "Keep bowls ready for mixing; snacks come together quickly once chutneys are set."
        };

        if (diet == MealDietType.NonVegetarian)
            intro += " If using poultry or fish, pat it dry before it touches the pan so it browns instead of boiling.";

        var midA = (n % 3) switch
        {
            0 => "Sauté aromatics until fragrant, then add ground spices and cook off the raw taste for a minute.",
            1 => "Layer wet and dry ingredients gradually so nothing sticks or clumps at the bottom.",
            _ => "Fold the mixture often so heat distributes evenly and colors stay bright."
        };

        var midB = (n % 2) == 0
            ? "Add liquids in small pours, scrape fond from the pan, and keep the flame steady."
            : "Cover partially so steam softens the core without turning everything mushy.";

        var midC = cat switch
        {
            RecipeCategoryKind.Breakfast => "Finish with a tempering or fresh toppings so crunch balances the base.",
            RecipeCategoryKind.Lunch => "Simmer until lentils or curry thicken; taste salt and sour before plating.",
            RecipeCategoryKind.Dinner => "Let complex sauces rest off heat for two minutes so flavors meld.",
            _ => "Toss chaat components at the last second so textures stay crisp."
        };

        var plate = $"Plate {title} with a fresh garnish, a wedge of lime if it fits the flavor, and serve immediately.";
        var store = $"If storing, cool fully, use an airtight container, and reheat gently to keep the best texture.";

        return
        [
            intro,
            "Measure spices and chop produce uniformly so cooking times stay predictable. Separate raw meat board from veg if applicable.",
            midA,
            midB,
            "Taste halfway: adjust salt, heat, or sourness with lemon, tamarind, or yogurt as needed.",
            midC,
            plate,
            store
        ];
    }
}
