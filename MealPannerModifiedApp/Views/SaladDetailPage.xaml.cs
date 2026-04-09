using MealPannerModifiedApp.Data;
using Microsoft.Maui.Controls.Shapes;

namespace MealPannerModifiedApp.Views;

public partial class SaladDetailPage : ContentPage, IQueryAttributable
{
    public SaladDetailPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("SaladId", out var raw) || raw?.ToString() is not string id)
            return;
        LoadSalad(Uri.UnescapeDataString(id));
    }

    private void LoadSalad(string? id)
    {
        if (string.IsNullOrEmpty(id))
            return;

        var salad = SaladCatalog.FindById(id);
        if (salad is null)
        {
            TitleLabel.Text = "Recipe not found";
            return;
        }

        Title = salad.Title.Length > 42 ? string.Concat(salad.Title.AsSpan(0, 42), "…") : salad.Title;
        HeroImage.Source = salad.ImageKey;
        TitleLabel.Text = salad.Title;
        var serve = string.IsNullOrEmpty(salad.ServesNote) ? "" : salad.ServesNote + " · ";
        MetaLabel.Text = $"{salad.Subtitle} · {serve}prep ~{salad.PrepMinutes} min";

        var res = Application.Current?.Resources;
        var accent = res?["AccentBright"] as Color ?? Colors.Goldenrod;
        var textPri = res?["TextPrimary"] as Color ?? Colors.Black;
        var surface = res?["SurfaceCard"] as Color ?? Colors.White;
        var border = res?["PanelBorder"] as Color ?? Colors.SandyBrown;

        IngredientsList.Children.Clear();
        foreach (var line in salad.Ingredients)
        {
            IngredientsList.Children.Add(new HorizontalStackLayout
            {
                Spacing = 8,
                Children =
                {
                    new Label { Text = "•", FontAttributes = FontAttributes.Bold, TextColor = accent },
                    new Label { Text = line, TextColor = textPri, VerticalOptions = LayoutOptions.Center }
                }
            });
        }

        StepsList.Children.Clear();
        var stepNo = 1;
        foreach (var step in salad.Steps)
        {
            var badge = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(18) },
                BackgroundColor = accent,
                HeightRequest = 36,
                WidthRequest = 36,
                VerticalOptions = LayoutOptions.Start,
                Content = new Label
                {
                    Text = stepNo.ToString(),
                    FontAttributes = FontAttributes.Bold,
                    TextColor = textPri,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };

            var row = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(14) },
                StrokeThickness = 1,
                Stroke = border,
                BackgroundColor = surface,
                Padding = new Thickness(14, 12),
                Content = new HorizontalStackLayout
                {
                    Spacing = 12,
                    Children =
                    {
                        badge,
                        new Label
                        {
                            Text = step,
                            LineBreakMode = LineBreakMode.WordWrap,
                            TextColor = textPri,
                            VerticalOptions = LayoutOptions.Center
                        }
                    }
                }
            };
            StepsList.Children.Add(row);
            stepNo++;
        }
    }
}
