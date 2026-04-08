namespace MealPlannerApp.Models;

public class ExerciseItem
{
    public string Name { get; set; } = string.Empty;
    public List<string> Steps { get; set; } = [];
    public int DurationMinutes { get; set; }
    public string TargetMuscle { get; set; } = string.Empty;
    public FitnessGoal Goal { get; set; }
    public string StepsSummary => string.Join(" ", Steps.Select(step => $"• {step}"));
}
