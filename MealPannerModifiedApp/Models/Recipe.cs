namespace MealPannerModifiedApp.Models;

public sealed class Recipe
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required RecipeCategoryKind Category { get; init; }
    /// <summary>MauiImage logical name without extension (e.g. recipe_dish_05).</summary>
    public required string ImageKey { get; init; }
    public required IReadOnlyList<string> Ingredients { get; init; }
    public required IReadOnlyList<string> Steps { get; init; }
    /// <summary>Hands-on cooking time estimate.</summary>
    public int PrepMinutes { get; init; }
    /// <summary>Total time from start to plate (prep + passive cooking).</summary>
    public int TotalTimeMinutes { get; init; }
    public MealDietType DietType { get; init; }
}
