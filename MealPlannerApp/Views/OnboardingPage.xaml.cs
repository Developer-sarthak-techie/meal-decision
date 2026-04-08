using MealPlannerApp.Helpers;
using MealPlannerApp.ViewModels;

namespace MealPlannerApp.Views;

public partial class OnboardingPage : ContentPage
{
    public OnboardingPage()
    {
        InitializeComponent();
        BindingContext = ServiceLocator.GetService<OnboardingViewModel>();
    }

    // What this function does:
    // Plays entry animation when onboarding screen appears.
    // Why it is needed:
    // Adds premium feel with smooth transition instead of abrupt UI load.
    // Input / Output:
    // Input: None.
    // Output: Visual fade/translate animation side-effect.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        RootLayout.TranslationY = 20;
        await Task.WhenAll(
            RootLayout.FadeTo(1, 350, Easing.CubicInOut),
            RootLayout.TranslateTo(0, 0, 350, Easing.CubicOut));
    }
}
