using MealPlannerApp.Models;

namespace MealPlannerApp.Data;

public static class AppSeedData
{
    public static readonly List<Recipe> Recipes =
    [
        new Recipe
        {
            DishName = "Moong Dal Chilla",
            IsVegetarian = true,
            MealTime = MealTime.Breakfast,
            Mood = MealMood.Light,
            Calories = 280,
            ProteinGrams = 18,
            HealthScore = 89,
            Ingredients = ["Moong dal", "Onion", "Coriander", "Spices"],
            QuickRecipe = "Blend soaked dal, add spices, and pan-cook thin chillas."
        },
        new Recipe
        {
            DishName = "Paneer Quinoa Bowl",
            IsVegetarian = true,
            MealTime = MealTime.Lunch,
            Mood = MealMood.Heavy,
            Calories = 520,
            ProteinGrams = 28,
            HealthScore = 84,
            Ingredients = ["Paneer", "Quinoa", "Bell peppers", "Olive oil"],
            QuickRecipe = "Saute paneer and vegetables, serve over cooked quinoa."
        },
        new Recipe
        {
            DishName = "Grilled Chicken Salad",
            IsVegetarian = false,
            MealTime = MealTime.Lunch,
            Mood = MealMood.Light,
            Calories = 350,
            ProteinGrams = 33,
            HealthScore = 91,
            Ingredients = ["Chicken breast", "Lettuce", "Cucumber", "Lemon dressing"],
            QuickRecipe = "Grill chicken, slice and toss with greens and lemon dressing."
        },
        new Recipe
        {
            DishName = "Egg Bhurji with Roti",
            IsVegetarian = false,
            MealTime = MealTime.Breakfast,
            Mood = MealMood.Heavy,
            Calories = 430,
            ProteinGrams = 24,
            HealthScore = 78,
            Ingredients = ["Eggs", "Onion", "Tomato", "Whole wheat roti"],
            QuickRecipe = "Cook masala bhurji and pair with fresh rotis."
        },
        new Recipe
        {
            DishName = "Dal Soup and Sauteed Veggies",
            IsVegetarian = true,
            MealTime = MealTime.Dinner,
            Mood = MealMood.Light,
            Calories = 310,
            ProteinGrams = 16,
            HealthScore = 88,
            Ingredients = ["Masoor dal", "Broccoli", "Carrot", "Garlic"],
            QuickRecipe = "Prepare thin dal soup and lightly saute vegetables."
        },
        new Recipe
        {
            DishName = "Chicken Stir Fry Rice Bowl",
            IsVegetarian = false,
            MealTime = MealTime.Dinner,
            Mood = MealMood.Heavy,
            Calories = 560,
            ProteinGrams = 38,
            HealthScore = 82,
            Ingredients = ["Chicken thigh", "Brown rice", "Beans", "Soy sauce"],
            QuickRecipe = "Stir fry chicken and vegetables, serve over rice."
        }
    ];

    public static readonly List<ExerciseItem> Exercises =
    [
        new ExerciseItem
        {
            Name = "Brisk Walk",
            DurationMinutes = 25,
            TargetMuscle = "Full Body",
            Goal = FitnessGoal.LoseWeight,
            Steps = ["Warm up for 3 minutes", "Walk fast for 20 minutes", "Cool down and stretch"]
        },
        new ExerciseItem
        {
            Name = "Push-Up Circuit",
            DurationMinutes = 20,
            TargetMuscle = "Chest and Triceps",
            Goal = FitnessGoal.BuildMuscle,
            Steps = ["3 sets of 10 push-ups", "Rest 45 seconds", "Finish with plank hold"]
        },
        new ExerciseItem
        {
            Name = "Squat and Lunge Mix",
            DurationMinutes = 25,
            TargetMuscle = "Legs and Glutes",
            Goal = FitnessGoal.AthleticBody,
            Steps = ["Bodyweight squats 15 reps x 3", "Alternating lunges 12 reps x 3", "Hip mobility stretch"]
        },
        new ExerciseItem
        {
            Name = "Resistance Band Rows",
            DurationMinutes = 18,
            TargetMuscle = "Back",
            Goal = FitnessGoal.GainWeight,
            Steps = ["Anchor band", "Perform 4 sets x 12 reps", "Slow eccentric motion"]
        },
        new ExerciseItem
        {
            Name = "Sun Salutation Flow",
            DurationMinutes = 15,
            TargetMuscle = "Core and Mobility",
            Goal = FitnessGoal.StayFit,
            Steps = ["Do 6 rounds of Surya Namaskar", "Breathe deeply", "Relax in child pose"]
        }
    ];

    public static readonly List<FoodLibraryItem> Foods =
    [
        new FoodLibraryItem
        {
            Name = "Banana",
            Image = "https://images.unsplash.com/photo-1603833665858-e61d17a86224?w=800",
            Benefits = "Fast energy, potassium rich, supports workout recovery.",
            BestTimeToEat = "Pre-workout or breakfast"
        },
        new FoodLibraryItem
        {
            Name = "Greek Yogurt",
            Image = "https://images.unsplash.com/photo-1488477181946-6428a0291777?w=800",
            Benefits = "High protein, gut-friendly probiotics, keeps you full longer.",
            BestTimeToEat = "Evening snack"
        },
        new FoodLibraryItem
        {
            Name = "Almonds",
            Image = "https://images.unsplash.com/photo-1508747703725-719777637510?w=800",
            Benefits = "Healthy fats, vitamin E, helps satiety and heart health.",
            BestTimeToEat = "Mid-morning"
        }
    ];

    public static readonly List<JourneyTask> BaseJourneyTasks =
        Enumerable.Range(1, 21).SelectMany(day => new List<JourneyTask>
        {
            new() { Day = day, Type = "Meal", Title = $"Follow curated meal plan for Day {day}" },
            new() { Day = day, Type = "Exercise", Title = $"Complete workout session for Day {day}" }
        }).ToList();
}
