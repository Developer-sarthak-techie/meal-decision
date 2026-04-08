using MealPannerModifiedApp.Data;
using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Views;

public partial class RecipeListPage : ContentPage, IQueryAttributable
{
    public RecipeListPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("Category", out var raw) || raw?.ToString() is not string s)
            return;
        if (Enum.TryParse(s, out RecipeCategoryKind c))
            ApplyCategory(c);
    }

    private void ApplyCategory(RecipeCategoryKind category)
    {
        HeaderLabel.Text = $"{category} · {RecipeCatalog.CountInCategory(category)} recipes";
        RecipeCollection.ItemsSource = RecipeCatalog.ByCategory[category];
    }

    private async void OnRecipeSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Recipe r)
            return;
        RecipeCollection.SelectedItem = null;
        await Shell.Current.GoToAsync($"{nameof(RecipeDetailPage)}?RecipeId={Uri.EscapeDataString(r.Id)}");
    }
}
