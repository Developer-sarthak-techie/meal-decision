namespace MealPannerModifiedApp.Models;

/// <summary>
/// General self-care education entry — not a diagnosis or prescription.
/// </summary>
public sealed class HomeRemedy
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string AudienceLabel { get; init; }
    public required string Category { get; init; }
    /// <summary>Lowercased blob for symptom / synonym search.</summary>
    public required string SearchText { get; init; }
    public required IReadOnlyList<string> Overview { get; init; }
    public required IReadOnlyList<string> SelfCare { get; init; }
    public required IReadOnlyList<string> HydrationAndFood { get; init; }
    public required IReadOnlyList<string> ComfortMeasures { get; init; }
    public required IReadOnlyList<string> WhatToAvoid { get; init; }
    public required IReadOnlyList<string> SeeDoctorPromptly { get; init; }
    /// <summary>How this entry relates to major public patient-education programs.</summary>
    public required string SourceAlignmentNote { get; init; }
}
