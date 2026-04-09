using MealPannerModifiedApp.Data;
using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Views;

public partial class PantryPage : ContentPage
{
    public PantryPage()
    {
        InitializeComponent();
        MealTypePicker.ItemsSource = new[] { "Breakfast", "Lunch", "Dinner", "Snacks" };
        MealTypePicker.SelectedIndex = 0;
        DietPicker.ItemsSource = new[] { "Vegetarian focus", "Non-vegetarian focus", "Show both" };
        DietPicker.SelectedIndex = 2;
    }

    private void OnFindClicked(object? sender, EventArgs e)
    {
        var pantry = PantryEditor.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(pantry))
        {
            ResultHintLabel.IsVisible = true;
            ResultHintLabel.Text = "Add at least a few pantry items (e.g. rice, onion, oil).";
            ResultCollection.ItemsSource = null;
            return;
        }

        var mealIdx = MealTypePicker.SelectedIndex >= 0 ? MealTypePicker.SelectedIndex : 0;
        if (mealIdx > 3) mealIdx = 0;
        var meal = (RecipeCategoryKind)mealIdx;
        var dietIdx = DietPicker.SelectedIndex >= 0 ? DietPicker.SelectedIndex : 2;
        var diet = dietIdx switch
        {
            0 => MealDietPreference.Veg,
            1 => MealDietPreference.NonVeg,
            _ => MealDietPreference.Mix
        };

        var results = PantryRecipeMatcher.Match(meal, diet, pantry);
        if (results.Count == 0)
            results = PantryRecipeMatcher.Match(meal, diet, pantry, maxResults: 120, minimumMatchFraction: 0.12);

        ResultCollection.ItemsSource = results;

        if (results.Count == 0)
        {
            ResultHintLabel.IsVisible = true;
            ResultHintLabel.Text = "No strong matches — try more staples (salt, oil, onion, rice) or a different meal type.";
        }
        else
        {
            ResultHintLabel.IsVisible = true;
            ResultHintLabel.Text = results.Count >= 100
                ? "Showing top 100 matches sorted by fit. Tap a card for full recipe."
                : $"{results.Count} matches. Tap a card for full recipe.";
        }
    }

    private async void OnRecipeSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not PantryMatchResult row)
            return;
        ResultCollection.SelectedItem = null;
        await Shell.Current.GoToAsync($"{nameof(RecipeDetailPage)}?RecipeId={Uri.EscapeDataString(row.Recipe.Id)}");
    }
}
