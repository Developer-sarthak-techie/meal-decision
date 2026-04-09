namespace MealPannerModifiedApp.Models;

/// <summary>Fresh salad with full ingredient list and numbered prep steps.</summary>
public sealed class SaladRecipe
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public required string ImageKey { get; init; }
    public int PrepMinutes { get; init; }
    /// <summary>Short note e.g. "Serves 2 as a side".</summary>
    public string ServesNote { get; init; } = "";
    public required IReadOnlyList<string> Ingredients { get; init; }
    public required IReadOnlyList<string> Steps { get; init; }
}
