using MealPannerModifiedApp.Data;
using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Views;

public partial class HomeRemediesPage : ContentPage
{
    public HomeRemediesPage()
    {
        InitializeComponent();
        CatalogHint.Text =
            $"Search by symptom (fever, nausea, cough…) or browse {HomeRemedyCatalog.TotalRemedyCount} conservative wellness entries (including adult / older adult / child-aware variants). This is education — not a diagnosis. Confirm with MedlinePlus, NHS, WHO, or your clinician.";
        ApplyFilter(string.Empty);
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e) => ApplyFilter(e.NewTextValue ?? string.Empty);

    private void OnSearchPressed(object? sender, EventArgs e) => ApplyFilter(RemedySearch.Text ?? string.Empty);

    private void ApplyFilter(string query)
    {
        RemedyCollection.ItemsSource = HomeRemedyCatalog.Search(query, maxResults: 250);
    }

    private async void OnRemedySelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not HomeRemedy r)
            return;
        RemedyCollection.SelectedItem = null;
        await Shell.Current.GoToAsync($"{nameof(HomeRemedyDetailPage)}?RemedyId={Uri.EscapeDataString(r.Id)}");
    }

    private static Task OpenUrl(string url) =>
        Launcher.Default.OpenAsync(new Uri(url));

    private void OnOpenMedlinePlus(object? sender, EventArgs e) =>
        _ = OpenUrl("https://medlineplus.gov/");

    private void OnOpenWho(object? sender, EventArgs e) =>
        _ = OpenUrl("https://www.who.int/news-room/fact-sheets/");

    private void OnOpenNhs(object? sender, EventArgs e) =>
        _ = OpenUrl("https://www.nhs.uk/conditions/");
}
