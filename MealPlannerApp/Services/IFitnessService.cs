using MealPlannerApp.Models;

namespace MealPlannerApp.Services;

public interface IFitnessService
{
    double CalculateBmi(double weightKg, double heightMeters);
    int CalculateDailyCaloriesTarget(UserProfile profile);
    string BuildGoalSummary(UserProfile profile, double bmi);
    Task<List<ExerciseItem>> GetExercisesForGoalAsync(FitnessGoal goal);
}
