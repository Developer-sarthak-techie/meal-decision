using MealPlannerApp.Data;
using MealPlannerApp.Models;

namespace MealPlannerApp.Services;

public class FitnessService : IFitnessService
{
    // What this function does:
    // Calculates BMI using weight and height.
    // Why it is needed:
    // BMI is a core health KPI displayed on dashboard and used for personalized guidance.
    // Input / Output:
    // Input: weight in kilograms and height in meters.
    // Output: Rounded BMI value with two decimals.
    public double CalculateBmi(double weightKg, double heightMeters)
    {
        if (heightMeters <= 0)
        {
            return 0;
        }

        return Math.Round(weightKg / (heightMeters * heightMeters), 2);
    }

    // What this function does:
    // Estimates daily calorie target based on age, weight, and user fitness goal.
    // Why it is needed:
    // Provides practical numeric target for diet planning and progress tracking.
    // Input / Output:
    // Input: User profile details.
    // Output: Estimated target calories per day.
    public int CalculateDailyCaloriesTarget(UserProfile profile)
    {
        var baseCalories = 10 * profile.WeightKg + 6.25 * (profile.HeightMeters * 100) - 5 * profile.Age + 5;

        var adjusted = profile.Goal switch
        {
            FitnessGoal.LoseWeight => baseCalories - 400,
            FitnessGoal.GainWeight => baseCalories + 350,
            FitnessGoal.BuildMuscle => baseCalories + 250,
            FitnessGoal.StayFit => baseCalories,
            FitnessGoal.AthleticBody => baseCalories + 150,
            _ => baseCalories
        };

        return (int)Math.Round(adjusted);
    }

    // What this function does:
    // Creates a readable one-line summary for user's current goal and BMI state.
    // Why it is needed:
    // Dashboard needs an actionable insight instead of only numeric values.
    // Input / Output:
    // Input: Profile and calculated BMI.
    // Output: User-friendly recommendation text.
    public string BuildGoalSummary(UserProfile profile, double bmi)
    {
        var bmiStatus = bmi switch
        {
            < 18.5 => "underweight",
            < 25 => "in a healthy BMI range",
            < 30 => "overweight",
            _ => "in the obesity range"
        };

        return $"Goal: {profile.Goal}. Current BMI indicates you are {bmiStatus}. Stay consistent for 21 days.";
    }

    // What this function does:
    // Returns exercise modules mapped to a selected goal.
    // Why it is needed:
    // Powers the workout page and daily journey assignments.
    // Input / Output:
    // Input: Fitness goal.
    // Output: List of matching exercises.
    public Task<List<ExerciseItem>> GetExercisesForGoalAsync(FitnessGoal goal)
    {
        return Task.FromResult(AppSeedData.Exercises.Where(ex => ex.Goal == goal).ToList());
    }
}
