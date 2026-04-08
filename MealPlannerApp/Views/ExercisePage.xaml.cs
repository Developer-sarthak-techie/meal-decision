using MealPlannerApp.Helpers;
using MealPlannerApp.ViewModels;

namespace MealPlannerApp.Views;

public partial class ExercisePage : ContentPage
{
    private readonly ExerciseViewModel _viewModel;

    public ExercisePage()
    {
        InitializeComponent();
        _viewModel = ServiceLocator.GetService<ExerciseViewModel>();
        BindingContext = _viewModel;
    }

    // What this function does:
    // Fetches exercises when page becomes visible.
    // Why it is needed:
    // Maintains real-time goal alignment if user changes settings.
    // Input / Output:
    // Input: None.
    // Output: Updated exercise cards.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadExercisesAsync();
    }
}
