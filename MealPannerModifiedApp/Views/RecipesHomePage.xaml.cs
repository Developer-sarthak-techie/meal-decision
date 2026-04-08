using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Views;

public partial class RecipesHomePage : ContentPage
{
    public RecipesHomePage()
    {
        InitializeComponent();
    }

    private Task GoCategory(RecipeCategoryKind c) =>
        Shell.Current.GoToAsync($"{nameof(RecipeListPage)}?Category={c}");

    private void OnSnacksClicked(object? sender, EventArgs e) => _ = GoCategory(RecipeCategoryKind.Snacks);

    private void OnLunchClicked(object? sender, EventArgs e) => _ = GoCategory(RecipeCategoryKind.Lunch);

    private void OnBreakfastClicked(object? sender, EventArgs e) => _ = GoCategory(RecipeCategoryKind.Breakfast);

    private void OnDinnerClicked(object? sender, EventArgs e) => _ = GoCategory(RecipeCategoryKind.Dinner);
}
