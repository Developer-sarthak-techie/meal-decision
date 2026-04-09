namespace MealPannerModifiedApp.Models;

/// <summary>
/// Educational spice/herb entry — not medical advice or a treatment recommendation.
/// </summary>
public sealed class SpiceEntry
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Category { get; init; }
    /// <summary>Lowercased blob for search (name, category, keywords).</summary>
    public required string SearchText { get; init; }
    public required IReadOnlyList<string> Benefits { get; init; }
    public required IReadOnlyList<string> HealthUses { get; init; }
    public required string EvidenceNote { get; init; }
}
