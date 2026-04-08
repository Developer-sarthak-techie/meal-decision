namespace MealPlannerApp.Models;

public class MealPlanItem
{
    public MealTime TimeSlot { get; set; }
    public Recipe Recipe { get; set; } = new();
    public string TimeSlotLabel => TimeSlot.ToString();
}
