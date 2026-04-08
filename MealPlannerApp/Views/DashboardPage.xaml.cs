using MealPlannerApp.Helpers;
using MealPlannerApp.ViewModels;

namespace MealPlannerApp.Views;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage()
    {
        InitializeComponent();
        _viewModel = ServiceLocator.GetService<DashboardViewModel>();
        BindingContext = _viewModel;
    }

    // What this function does:
    // Refreshes dashboard metrics whenever user visits the page.
    // Why it is needed:
    // Ensures BMI, calories, streak, and progress always reflect latest user actions.
    // Input / Output:
    // Input: None.
    // Output: Updated dashboard cards and progress widgets.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDashboardAsync();
    }
}
