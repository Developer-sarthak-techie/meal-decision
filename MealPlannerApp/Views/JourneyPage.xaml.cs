using MealPlannerApp.Helpers;
using MealPlannerApp.Models;
using MealPlannerApp.ViewModels;

namespace MealPlannerApp.Views;

public partial class JourneyPage : ContentPage
{
    private readonly JourneyViewModel _viewModel;

    public JourneyPage()
    {
        InitializeComponent();
        _viewModel = ServiceLocator.GetService<JourneyViewModel>();
        BindingContext = _viewModel;
    }

    // What this function does:
    // Reloads journey tasks and tracker stats when screen opens.
    // Why it is needed:
    // Keeps streak and progress accurate after task completion toggles.
    // Input / Output:
    // Input: None.
    // Output: Refreshed tasks and tracker widgets.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadTasksAsync();
    }

    // What this function does:
    // Handles checkbox toggle and forwards selected task to ViewModel.
    // Why it is needed:
    // CheckBox in MAUI does not expose Command binding, so event handling is required.
    // Input / Output:
    // Input: CheckedChanged event sender and args.
    // Output: Persists task state and refreshes progress UI.
    private async void OnTaskCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (sender is not CheckBox { BindingContext: JourneyTask task })
        {
            return;
        }

        await _viewModel.ToggleTaskAsync(task);
    }
}
