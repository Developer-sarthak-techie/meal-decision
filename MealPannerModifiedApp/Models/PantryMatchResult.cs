namespace MealPannerModifiedApp.Models;

/// <summary>Recipe ranked by how well pantry items cover its ingredient list.</summary>
public sealed class PantryMatchResult
{
    public required Recipe Recipe { get; init; }
    /// <summary>0–100: fraction of ingredient lines matched by at least one pantry token.</summary>
    public double MatchPercent { get; init; }
    public int IngredientLinesMatched { get; init; }
    public int IngredientLineCount { get; init; }
    public required IReadOnlyList<string> MissingIngredientHints { get; init; }

    public string MatchSummary =>
        $"{MatchPercent:0.#}% pantry match · {IngredientLinesMatched}/{IngredientLineCount} ingredient lines";

    public string MissingPreview =>
        MissingIngredientHints.Count > 0
            ? "Likely still need: " + MissingIngredientHints[0]
            : "Most ingredient groups look covered — open for steps.";
}
