using MealPannerModifiedApp.Data;
using MealPannerModifiedApp.Models;
using Microsoft.Maui.Controls.Shapes;

namespace MealPannerModifiedApp.Views;

public partial class RecipeDetailPage : ContentPage, IQueryAttributable
{
    public RecipeDetailPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("RecipeId", out var raw) || raw?.ToString() is not string id)
            return;
        LoadRecipe(id);
    }

    private void LoadRecipe(string? id)
    {
        if (string.IsNullOrEmpty(id))
            return;

        var recipe = RecipeCatalog.FindById(Uri.UnescapeDataString(id));
        if (recipe is null)
        {
            TitleLabel.Text = "Recipe not found";
            return;
        }

        Title = recipe.Title.Length > 40 ? string.Concat(recipe.Title.AsSpan(0, 40), "…") : recipe.Title;
        HeroImage.Source = recipe.ImageKey;
        TitleLabel.Text = recipe.Title;
        MetaLabel.Text =
            $"{recipe.Category} · {(recipe.DietType == MealDietType.Vegetarian ? "Veg" : "Non-veg")} · active ~{recipe.PrepMinutes} min · total ~{recipe.TotalTimeMinutes} min";

        var res = Application.Current?.Resources;
        var accent = res?["AccentBright"] as Color ?? Colors.Goldenrod;
        var textPri = res?["TextPrimary"] as Color ?? Colors.Black;
        var surface = res?["SurfaceCard"] as Color ?? Colors.White;
        var border = res?["PanelBorder"] as Color ?? Colors.SandyBrown;

        IngredientsList.Children.Clear();
        foreach (var line in recipe.Ingredients)
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
        foreach (var step in recipe.Steps)
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
