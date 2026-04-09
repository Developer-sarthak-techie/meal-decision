namespace MealPannerModifiedApp.Models;

/// <summary>Summer-friendly cold drink / smoothie with full ingredient and step lists.</summary>
public sealed class ShakeRecipe
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    /// <summary>MauiImage logical name without extension.</summary>
    public required string ImageKey { get; init; }
    public int PrepMinutes { get; init; }
    public required IReadOnlyList<string> Ingredients { get; init; }
    public required IReadOnlyList<string> Steps { get; init; }
}
