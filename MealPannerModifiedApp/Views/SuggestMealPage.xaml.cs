using MealPannerModifiedApp.Data;
using MealPannerModifiedApp.Models;
using Microsoft.Maui.Controls.Shapes;

namespace MealPannerModifiedApp.Views;

public partial class SuggestMealPage : ContentPage
{
    public SuggestMealPage()
    {
        InitializeComponent();
        DietPicker.ItemsSource = new[] { "Vegetarian", "Non-vegetarian", "Mix (anything)" };
        DietPicker.SelectedIndex = 2;
        MealPicker.ItemsSource = new[]
        {
            "Breakfast",
            "Lunch",
            "Dinner",
            "Snacks"
        };
        MealPicker.SelectedIndex = 0;
    }

    private void OnSuggestClicked(object? sender, EventArgs e)
    {
        if (DietPicker.SelectedIndex < 0 || MealPicker.SelectedIndex < 0)
            return;

        var pref = DietPicker.SelectedIndex switch
        {
            0 => MealDietPreference.Veg,
            1 => MealDietPreference.NonVeg,
            _ => MealDietPreference.Mix
        };
        var meal = MealPicker.SelectedIndex switch
        {
            0 => RecipeCategoryKind.Breakfast,
            1 => RecipeCategoryKind.Lunch,
            2 => RecipeCategoryKind.Dinner,
            _ => RecipeCategoryKind.Snacks
        };

        var recipe = RecipeCatalog.SuggestRecipe(pref, meal);
        if (recipe is null)
        {
            ResultScroll.IsVisible = true;
            ClearResult();
            ResultTitle.Text = "No exact match";
            ResultMeta.Text =
                "Try “Mix” or switch meal type — catalogue skews vegetarian in some slots.";
            ResultNoobIntro.Text = string.Empty;
            return;
        }

        ShowRecipe(recipe);
    }

    private void ClearResult()
    {
        IngredientsBox.Children.Clear();
        StepsBox.Children.Clear();
        ResultNoobIntro.Text = string.Empty;
        ResultImage.Source = null;
    }

    private void ShowRecipe(Recipe recipe)
    {
        ClearResult();

        var res = Application.Current?.Resources;
        var accent = res?["AccentBright"] as Color ?? Colors.Goldenrod;
        var textPri = res?["TextPrimary"] as Color ?? Colors.Black;
        var textSec = res?["TextSecondary"] as Color ?? Colors.Gray;
        var surface = res?["SurfaceCard"] as Color ?? Colors.White;
        var border = res?["PanelBorder"] as Color ?? Colors.SandyBrown;

        ResultScroll.IsVisible = true;
        ResultImage.Source = recipe.ImageKey;
        ResultTitle.Text = recipe.Title;
        var dietLabel = recipe.DietType == MealDietType.Vegetarian ? "Vegetarian" : "Non-vegetarian";
        ResultMeta.Text =
            $"{recipe.Category} · {dietLabel} · active cooking ~{recipe.PrepMinutes} min · total ~{recipe.TotalTimeMinutes} min";
        ResultNoobIntro.Text =
            $"Beginners: work on medium heat unless a step says high. Internal temperature for poultry should hit 74°C / 165°F if you use meat — when in doubt, cook a minute longer and rest before slicing.";

        foreach (var line in recipe.Ingredients)
        {
            IngredientsBox.Children.Add(new HorizontalStackLayout
            {
                Spacing = 8,
                Children =
                {
                    new Label { Text = "•", FontAttributes = FontAttributes.Bold, TextColor = accent },
                    new Label { Text = line, TextColor = textPri, LineBreakMode = LineBreakMode.WordWrap }
                }
            });
        }

        var steps = RecipeCatalog.GetNoobFriendlySteps(recipe);
        var stepNo = 1;
        foreach (var step in steps)
        {
            var badge = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(18) },
                BackgroundColor = accent,
                HeightRequest = 36,
                WidthRequest = 40,
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

            StepsBox.Children.Add(new Border
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
                            TextColor = textPri
                        }
                    }
                }
            });
            stepNo++;
        }
    }
}
