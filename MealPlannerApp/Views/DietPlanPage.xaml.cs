using MealPlannerApp.Helpers;
using MealPlannerApp.ViewModels;

namespace MealPlannerApp.Views;

public partial class DietPlanPage : ContentPage
{
    private readonly DietPlanViewModel _viewModel;

    public DietPlanPage()
    {
        InitializeComponent();
        _viewModel = ServiceLocator.GetService<DietPlanViewModel>();
        BindingContext = _viewModel;
    }

    // What this function does:
    // Auto-loads diet plan each time the page becomes active.
    // Why it is needed:
    // Keeps meal recommendations fresh and aligned with latest goal settings.
    // Input / Output:
    // Input: None.
    // Output: Refreshed DailyPlan list rendered in UI.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.GeneratePlanAsync();
    }
}
