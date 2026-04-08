namespace MealPlannerApp.Models;

public class UserProfile
{
    public double HeightMeters { get; set; }
    public double WeightKg { get; set; }
    public int Age { get; set; }
    public FitnessGoal Goal { get; set; }
    public DietPreference DietPreference { get; set; }
}
