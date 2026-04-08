namespace MealPlannerApp.Models;

public class JourneyTask
{
    public int Day { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public string TaskId => $"{Day}-{Type}-{Title}";
}
