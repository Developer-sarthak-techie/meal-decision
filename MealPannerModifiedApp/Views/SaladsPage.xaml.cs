using MealPannerModifiedApp.Data;
using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Views;

public partial class SaladsPage : ContentPage
{
    public SaladsPage()
    {
        InitializeComponent();
        CountLabel.Text =
            $"{SaladCatalog.Count} salads with ingredient lists and numbered steps — from quick slaws to grain bowls.";
        SaladCollection.ItemsSource = SaladCatalog.All;
    }

    private async void OnSaladSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not SaladRecipe salad)
            return;
        SaladCollection.SelectedItem = null;
        await Shell.Current.GoToAsync($"{nameof(SaladDetailPage)}?SaladId={Uri.EscapeDataString(salad.Id)}");
    }
}
