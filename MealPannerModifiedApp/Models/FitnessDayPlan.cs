namespace MealPannerModifiedApp.Models;

public sealed class FitnessDayPlan
{
    public int DayNumber { get; init; }
    public required string Focus { get; init; }
    public required string Breakfast { get; init; }
    public required string Lunch { get; init; }
    public required string Dinner { get; init; }
    public required string Snack { get; init; }
    public int DailyCalories { get; init; }
    public int WaterMlTarget { get; init; }
}
