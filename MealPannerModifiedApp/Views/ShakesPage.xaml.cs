using MealPannerModifiedApp.Data;
using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Views;

public partial class ShakesPage : ContentPage
{
    public ShakesPage()
    {
        InitializeComponent();
        CountLabel.Text =
            $"{ShakeCatalog.Count} chilled drinks with full ingredient lists and numbered steps — perfect for hot days.";
        ShakeCollection.ItemsSource = ShakeCatalog.All;
    }

    private async void OnShakeSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not ShakeRecipe shake)
            return;
        ShakeCollection.SelectedItem = null;
        await Shell.Current.GoToAsync($"{nameof(ShakeDetailPage)}?ShakeId={Uri.EscapeDataString(shake.Id)}");
    }
}
