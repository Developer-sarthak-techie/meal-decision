using MealPlannerApp.Helpers;
using MealPlannerApp.ViewModels;

namespace MealPlannerApp.Views;

public partial class AajKyaBanayePage : ContentPage
{
    private readonly AajKyaBanayeViewModel _viewModel;

    public AajKyaBanayePage()
    {
        InitializeComponent();
        _viewModel = ServiceLocator.GetService<AajKyaBanayeViewModel>();
        BindingContext = _viewModel;
    }

    // What this function does:
    // Loads initial state and plays staged entrance animations for premium feel.
    // Why it is needed:
    // Adds designer-grade motion to make the app feel modern and polished.
    // Input / Output:
    // Input: None.
    // Output: Initialized view-model state and animated cards.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();

        RootLayout.Opacity = 0;
        HeroCard.TranslationY = 16;
        ControlCard.TranslationY = 24;
        SuggestionCard.TranslationY = 30;
        WeeklyCollection.TranslationY = 36;

        await Task.WhenAll(
            RootLayout.FadeTo(1, 280, Easing.CubicOut),
            HeroCard.TranslateTo(0, 0, 280, Easing.CubicOut));
        await ControlCard.TranslateTo(0, 0, 220, Easing.CubicOut);
        await SuggestionCard.TranslateTo(0, 0, 220, Easing.CubicOut);
        await WeeklyCollection.TranslateTo(0, 0, 220, Easing.CubicOut);
    }
}
