using MealPlannerApp.Helpers;
using MealPlannerApp.ViewModels;

namespace MealPlannerApp.Views;

public partial class FoodLibraryPage : ContentPage
{
    private readonly FoodLibraryViewModel _viewModel;

    public FoodLibraryPage()
    {
        InitializeComponent();
        _viewModel = ServiceLocator.GetService<FoodLibraryViewModel>();
        BindingContext = _viewModel;
    }

    // What this function does:
    // Loads food cards each time page appears.
    // Why it is needed:
    // Ensures visual library is always available even after app state changes.
    // Input / Output:
    // Input: None.
    // Output: Updated Foods collection for grid display.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadFoodsAsync();
    }
}
