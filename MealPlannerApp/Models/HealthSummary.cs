namespace MealPlannerApp.Models;

public class HealthSummary
{
    public double Bmi { get; set; }
    public int DailyCaloriesTarget { get; set; }
    public string GoalSummary { get; set; } = string.Empty;
    public int CurrentStreak { get; set; }
    public double JourneyProgress { get; set; }
}
