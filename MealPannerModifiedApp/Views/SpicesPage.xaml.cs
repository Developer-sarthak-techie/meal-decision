using MealPannerModifiedApp.Data;
using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Views;

public partial class SpicesPage : ContentPage
{
    public SpicesPage()
    {
        InitializeComponent();
        CatalogHint.Text =
            $"Browse {SpiceCatalog.TotalCount}+ entries (including whole / ground / toasted variants). "
            + "Use search for names or ideas like “leaf”, “pepper”, “digestion”. Educational only — not medical advice.";
        ApplyFilter(string.Empty);
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e) =>
        ApplyFilter(e.NewTextValue ?? string.Empty);

    private void OnSearchPressed(object? sender, EventArgs e) => ApplyFilter(SpiceSearch.Text ?? string.Empty);

    private void ApplyFilter(string query) =>
        SpiceCollection.ItemsSource = SpiceCatalog.Search(query, maxResults: 350);

    private async void OnSpiceSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not SpiceEntry s)
            return;
        SpiceCollection.SelectedItem = null;
        await Shell.Current.GoToAsync($"{nameof(SpiceDetailPage)}?SpiceId={Uri.EscapeDataString(s.Id)}");
    }
}
