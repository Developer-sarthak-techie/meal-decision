using MealPlannerApp.Helpers;
using MealPlannerApp.ViewModels;

namespace MealPlannerApp.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;

    public SettingsPage()
    {
        InitializeComponent();
        _viewModel = ServiceLocator.GetService<SettingsViewModel>();
        BindingContext = _viewModel;
    }

    // What this function does:
    // Loads persisted settings each time the page appears.
    // Why it is needed:
    // Keeps controls synchronized with latest profile values.
    // Input / Output:
    // Input: None.
    // Output: Updated picker values and status label.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSettingsAsync();
    }
}
