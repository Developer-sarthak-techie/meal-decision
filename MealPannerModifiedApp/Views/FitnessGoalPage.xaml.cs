using MealPannerModifiedApp.Data;
using MealPannerModifiedApp.Infrastructure;
using MealPannerModifiedApp.Models;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Devices;

namespace MealPannerModifiedApp.Views;

public partial class FitnessGoalPage : ContentPage
{
    private readonly Border[] _goalTiles = new Border[7];
    private int _selectedGoalIndex;
    private int _selectedDietIndex = 2;
    private CancellationTokenSource? _heroAnimCts;

    private static readonly (string Emoji, string Line)[] GoalMoods =
    [
        ("🎯", "Get fit"),
        ("📉", "Lose weight"),
        ("📈", "Gain weight"),
        ("💪", "Build muscle"),
        ("🏃", "Endurance"),
        ("⚖️", "Recomp"),
        ("❤️", "Maintain")
    ];

    private FitnessGoalType _latestGoal = FitnessGoalType.GetFit;
    private MealDietPreference _latestDiet = MealDietPreference.Mix;
    private double _latestWeightKg;
    private double _latestHeightCm;
    private double _latestBmi;
    private IReadOnlyList<string> _latestTips = [];
    private IReadOnlyList<FitnessDayPlan> _latestPlan = [];

    public FitnessGoalPage()
    {
        InitializeComponent();
        BuildGoalTiles();
        UpdateLiveBmiPreview();
        _ = SelectDietVisualAsync(_selectedDietIndex, animate: false);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _heroAnimCts?.Cancel();
        _heroAnimCts = new CancellationTokenSource();
        _ = RunHeroEntranceAsync(_heroAnimCts.Token);
        _ = PulseHeroSparkleAsync(_heroAnimCts.Token);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _heroAnimCts?.Cancel();
        _heroAnimCts?.Dispose();
        _heroAnimCts = null;
    }

    private async Task RunHeroEntranceAsync(CancellationToken ct)
    {
        HeroBlock.TranslationY = 28;
        HeroBlock.Opacity = 0;
        await Task.WhenAll(
            HeroBlock.FadeTo(1, 520, Easing.CubicOut),
            HeroBlock.TranslateTo(0, 0, 540, Easing.CubicOut));
        if (ct.IsCancellationRequested) return;
    }

    private async Task PulseHeroSparkleAsync(CancellationToken ct)
    {
        try
        {
            await Task.Delay(300, ct);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        while (!ct.IsCancellationRequested)
        {
            await HeroEmoji.ScaleTo(1.12, 650, Easing.SinInOut);
            if (ct.IsCancellationRequested) break;
            await HeroEmoji.ScaleTo(1.0, 650, Easing.SinInOut);
            if (ct.IsCancellationRequested) break;
            await HeroEmoji.RotateTo(8, 400, Easing.CubicInOut);
            if (ct.IsCancellationRequested) break;
            await HeroEmoji.RotateTo(-8, 500, Easing.CubicInOut);
            if (ct.IsCancellationRequested) break;
            await HeroEmoji.RotateTo(0, 400, Easing.CubicInOut);
            if (ct.IsCancellationRequested) break;
            await Task.Delay(600, ct);
        }
    }

    private void BuildGoalTiles()
    {
        GoalFlex.Children.Clear();
        for (var i = 0; i < GoalMoods.Length; i++)
        {
            var idx = i;
            var (emoji, line) = GoalMoods[i];
            var tile = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(22) },
                BackgroundColor = GlassFill(false),
                Stroke = new SolidColorBrush(GlassStrokeColor()),
                StrokeThickness = 1,
                Padding = new Thickness(10, 8),
                Margin = new Thickness(5),
                WidthRequest = 110,
                HeightRequest = 102,
                Content = new VerticalStackLayout
                {
                    Spacing = 4,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = emoji,
                            FontSize = 30,
                            HorizontalTextAlignment = TextAlignment.Center
                        },
                        new Label
                        {
                            Text = line,
                            FontSize = 11,
                            LineBreakMode = LineBreakMode.WordWrap,
                            MaxLines = 2,
                            HorizontalTextAlignment = TextAlignment.Center,
                            TextColor = TryColor("TextPrimary", Colors.Black)
                        }
                    }
                }
            };
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (_, _) => await SelectGoalAsync(idx);
            tile.GestureRecognizers.Add(tap);
            _goalTiles[i] = tile;
            GoalFlex.Children.Add(tile);
        }

        ApplyGoalVisualState(_selectedGoalIndex, animate: false);
    }

    private static Color TryColor(string key, Color fallback)
    {
        if (Application.Current?.Resources.TryGetValue(key, out var o) == true && o is Color c)
            return c;
        return fallback;
    }

    private Color GlassFill(bool selected)
    {
        var key = selected ? "AccentSoft" : "SurfaceCard";
        var baseC = TryColor(key, Color.FromArgb("#FFFFFF"));
        return baseC.WithAlpha(selected ? 0.58f : 0.40f);
    }

    private Color GlassStrokeColor()
    {
        var c = TryColor("AccentBright", Color.FromArgb("#FFC400"));
        return c.WithAlpha(0.55f);
    }

    private void ApplyGoalVisualState(int selectedIndex, bool animate)
    {
        for (var i = 0; i < _goalTiles.Length; i++)
        {
            var tile = _goalTiles[i];
            if (tile is null) continue;
            var sel = i == selectedIndex;
            tile.StrokeThickness = sel ? 2.5 : 1;
            tile.BackgroundColor = GlassFill(sel);
            tile.ZIndex = sel ? 1 : 0;

            if (animate)
                _ = AnimateGoalTileAsync(tile, sel);
            else
            {
                tile.Scale = sel ? 1.06 : 1;
                tile.Rotation = sel ? 1.5 : 0;
            }
        }
    }

    private static async Task AnimateGoalTileAsync(Border tile, bool selected)
    {
        var targetScale = selected ? 1.08 : 1.0;
        var targetRot = selected ? 2.0 : 0.0;
        await Task.WhenAll(
            tile.ScaleTo(targetScale, 280, Easing.SpringOut),
            tile.RotateTo(targetRot, 320, Easing.CubicOut));
    }

    private async Task SelectGoalAsync(int index)
    {
        if (index < 0 || index >= _goalTiles.Length || _selectedGoalIndex == index)
            return;

        _selectedGoalIndex = index;
        ApplyGoalVisualState(index, animate: true);
        TryHaptic();
        await Task.Delay(40);
    }

    private async void OnDietTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Border b) return;
        var idx = b == DietVegBorder ? 0 : b == DietNonVegBorder ? 1 : 2;
        await SelectDietVisualAsync(idx, animate: true);
    }

    private async Task SelectDietVisualAsync(int index, bool animate)
    {
        if (index < 0 || index > 2)
            return;

        _selectedDietIndex = index;
        var borders = new[] { DietVegBorder, DietNonVegBorder, DietMixBorder };
        for (var i = 0; i < borders.Length; i++)
        {
            var bor = borders[i];
            var sel = i == index;
            bor.StrokeThickness = sel ? 2.5 : 1;
            bor.BackgroundColor = GlassFill(sel);
            if (animate)
                await bor.ScaleTo(sel ? 1.07 : 1.0, 220, Easing.SpringOut);
            else
                bor.Scale = sel ? 1.05 : 1.0;
        }

        if (animate)
            TryHaptic();
    }

    private static void TryHaptic()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch
        {
            // ignore on unsupported platforms
        }
    }

    private void OnWeightSliderChanged(object? sender, ValueChangedEventArgs e)
    {
        WeightReadout.Text = $"{WeightSlider.Value:F1} kg";
        UpdateLiveBmiPreview();
    }

    private void OnHeightSliderChanged(object? sender, ValueChangedEventArgs e)
    {
        HeightReadout.Text = $"{HeightSlider.Value:F0} cm";
        UpdateLiveBmiPreview();
    }

    private async void OnSliderDragStarted(object? sender, EventArgs e)
    {
        if (BmiCrystalPreview is null) return;
        await BmiCrystalPreview.ScaleTo(1.02, 100);
    }

    private async void OnSliderDragCompleted(object? sender, EventArgs e)
    {
        if (BmiCrystalPreview is null) return;
        await BmiCrystalPreview.ScaleTo(1.05, 110, Easing.SpringOut);
        await BmiCrystalPreview.ScaleTo(1.0, 200, Easing.BounceOut);
        TryHaptic();
    }

    private void UpdateLiveBmiPreview()
    {
        var w = WeightSlider.Value;
        var h = HeightSlider.Value;
        WeightReadout.Text = $"{w:F1} kg";
        HeightReadout.Text = $"{h:F0} cm";
        var bmi = CalculateBmi(w, h);
        var band = BmiBand(bmi);
        BmiPreviewValue.Text = $"BMI {bmi:F1}";
        BmiPreviewBand.Text = band;
        BmiPreviewEmoji.Text = bmi switch
        {
            < 18.5 => "🌱",
            < 25 => "💚",
            < 30 => "⚡",
            _ => "🫶"
        };
    }

    private async void OnGenerateClicked(object? sender, EventArgs e)
    {
        var weightKg = WeightSlider.Value;
        var heightCm = HeightSlider.Value;

        if (weightKg < 25 || weightKg > 250 || heightCm < 120 || heightCm > 230)
        {
            await DisplayAlert("Almost there", "Keep weight and height inside the slider range.", "OK");
            return;
        }

        var goal = (FitnessGoalType)Math.Clamp(_selectedGoalIndex, 0, Enum.GetValues<FitnessGoalType>().Length - 1);
        var diet = _selectedDietIndex switch
        {
            0 => MealDietPreference.Veg,
            1 => MealDietPreference.NonVeg,
            _ => MealDietPreference.Mix
        };

        await GenerateButton.ScaleTo(0.95, 70, Easing.CubicOut);
        await GenerateButton.ScaleTo(1.0, 140, Easing.SpringOut);

        var bmi = CalculateBmi(weightKg, heightCm);
        var bmiBand = BmiBand(bmi);
        BmiLabel.Text = $"BMI: {bmi:F1} ({bmiBand}) ✨";
        ProfileSummaryLabel.Text =
            $"{GoalMoodEmoji(goal)} Goal: {GoalLabel(goal)} · {weightKg:F1} kg · {heightCm:F0} cm · {DietEmoji(diet)} {DietLabel(diet)}";

        _latestGoal = goal;
        _latestDiet = diet;
        _latestWeightKg = weightKg;
        _latestHeightCm = heightCm;
        _latestBmi = bmi;
        _latestTips = BuildTips(goal, bmi);
        _latestPlan = Build21DayPlan(goal, diet, weightKg, bmi);

        TipsCollection.ItemsSource = _latestTips;
        PlanCollection.ItemsSource = _latestPlan;
        ResultsPanel.IsVisible = true;
        ResultsPanel.Opacity = 0;
        ExportPdfButton.IsEnabled = _latestPlan.Count > 0;

        await ResultsPanel.FadeTo(1, 480, Easing.CubicOut);
        await Task.Delay(50);
        TryHaptic();
    }

    private static string GoalMoodEmoji(FitnessGoalType goal) => goal switch
    {
        FitnessGoalType.GetFit => "🎯",
        FitnessGoalType.LoseWeight => "📉",
        FitnessGoalType.GainWeight => "📈",
        FitnessGoalType.BuildMuscle => "💪",
        FitnessGoalType.ImproveEndurance => "🏃",
        FitnessGoalType.Recomposition => "⚖️",
        _ => "❤️"
    };

    private static string DietEmoji(MealDietPreference pref) => pref switch
    {
        MealDietPreference.Veg => "🥬",
        MealDietPreference.NonVeg => "🍗",
        _ => "🌈"
    };

    private async void OnExportPdfClicked(object? sender, EventArgs e)
    {
        if (_latestPlan.Count == 0)
        {
            await DisplayAlert("Nothing to export", "Craft your 21-day plan first.", "OK");
            return;
        }

        try
        {
            var fileName = $"FitnessPlan_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var outputPath = global::System.IO.Path.Combine(FileSystem.CacheDirectory, fileName);

            var bmiBand = BmiBand(_latestBmi);
            var generatedAt = DateTime.Now.ToString("dd MMM yyyy, HH:mm");

            FitnessPlanPdfExporter.Generate(
                outputPath,
                generatedAt,
                _latestGoal,
                _latestDiet,
                _latestWeightKg,
                _latestHeightCm,
                _latestBmi,
                bmiBand,
                _latestTips,
                _latestPlan,
                GoalLabel(_latestGoal),
                DietLabel(_latestDiet));

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Export Fitness Plan PDF",
                File = new ShareFile(outputPath)
            });
        }
        catch (Exception ex)
        {
            var detail = ex.InnerException != null ? $"{ex.Message} ({ex.InnerException.Message})" : ex.Message;
            await DisplayAlert("Export failed", $"Could not generate PDF: {detail}", "OK");
        }
    }

    private static double CalculateBmi(double weightKg, double heightCm)
    {
        var meters = heightCm / 100d;
        return weightKg / (meters * meters);
    }

    private static string BmiBand(double bmi)
    {
        if (bmi < 18.5) return "Underweight";
        if (bmi < 25) return "Healthy range";
        if (bmi < 30) return "Overweight";
        return "Obesity range";
    }

    private static string GoalLabel(FitnessGoalType goal) => goal switch
    {
        FitnessGoalType.GetFit => "Get fit",
        FitnessGoalType.LoseWeight => "Lose weight",
        FitnessGoalType.GainWeight => "Gain weight",
        FitnessGoalType.BuildMuscle => "Build muscle",
        FitnessGoalType.ImproveEndurance => "Improve endurance",
        FitnessGoalType.Recomposition => "Body recomposition",
        _ => "Maintain health"
    };

    private static string DietLabel(MealDietPreference pref) => pref switch
    {
        MealDietPreference.Veg => "Vegetarian",
        MealDietPreference.NonVeg => "Non-vegetarian",
        _ => "Mix"
    };

    private static IReadOnlyList<string> BuildTips(FitnessGoalType goal, double bmi)
    {
        var tips = new List<string>
        {
            "Sleep 7-8 hours consistently; recovery quality controls fat loss, strength, and cravings.",
            "Start each meal with protein + fiber (dal/eggs/chicken/tofu + vegetables) to improve satiety.",
            "Hydrate through the day, not all at once. Add electrolytes in hot weather or heavy workouts.",
            "Use a fixed meal window and avoid late-night heavy meals for better digestion."
        };

        switch (goal)
        {
            case FitnessGoalType.LoseWeight:
                tips.Add("Aim for a small calorie deficit (300-500 kcal/day), not crash dieting.");
                tips.Add("Walk 8k-10k steps daily and include 3 strength sessions/week to preserve muscle.");
                break;
            case FitnessGoalType.GainWeight:
                tips.Add("Add 300-450 kcal/day from quality foods (rice, potatoes, nuts, dairy, lean protein).");
                tips.Add("Eat 4-5 meals/day and include a protein snack before bed.");
                break;
            case FitnessGoalType.BuildMuscle:
                tips.Add("Target 1.6-2.2 g protein/kg body weight daily and train each muscle 2x/week.");
                tips.Add("Progressive overload: add reps/weight gradually each week.");
                break;
            case FitnessGoalType.ImproveEndurance:
                tips.Add("Include 3 zone-2 cardio sessions/week and one interval day.");
                tips.Add("On long training days, increase carbs and sodium.");
                break;
            case FitnessGoalType.Recomposition:
                tips.Add("Keep protein high, maintain calories near maintenance, and prioritize resistance training.");
                tips.Add("Track waist + photos weekly; scale weight alone can be misleading.");
                break;
            case FitnessGoalType.GetFit:
            case FitnessGoalType.MaintainHealth:
                tips.Add("Mix strength + mobility + cardio across the week to stay resilient.");
                tips.Add("Keep one fully restful day per week to avoid burnout.");
                break;
        }

        if (bmi >= 30) tips.Add("Consult a clinician before very intense plans; start with low-impact workouts.");
        if (bmi < 18.5) tips.Add("Prioritize adequate calories and nutrient density before aggressive cardio.");
        return tips;
    }

    private static IReadOnlyList<FitnessDayPlan> Build21DayPlan(
        FitnessGoalType goal,
        MealDietPreference diet,
        double weightKg,
        double bmi)
    {
        var calorieTarget = GoalCalories(goal, weightKg, bmi);
        var waterTargetMl = Math.Clamp((int)Math.Round(weightKg * 35), 1800, 4200);
        var list = new List<FitnessDayPlan>(21);

        for (var day = 1; day <= 21; day++)
        {
            var breakfast = PickRecipe(RecipeCategoryKind.Breakfast, diet, day * 17 + (int)goal * 13);
            var lunch = PickRecipe(RecipeCategoryKind.Lunch, diet, day * 19 + (int)goal * 7);
            var dinner = PickRecipe(RecipeCategoryKind.Dinner, diet, day * 23 + (int)goal * 11);
            var snack = PickRecipe(RecipeCategoryKind.Snacks, diet, day * 29 + (int)goal * 5);

            list.Add(new FitnessDayPlan
            {
                DayNumber = day,
                Focus = DailyFocus(goal, day),
                Breakfast = breakfast?.Title ?? "Protein-rich breakfast bowl",
                Lunch = lunch?.Title ?? "Balanced lunch plate",
                Dinner = dinner?.Title ?? "Light dinner with vegetables",
                Snack = snack?.Title ?? "Fruit + nuts snack",
                DailyCalories = calorieTarget,
                WaterMlTarget = waterTargetMl
            });
        }

        return list;
    }

    private static Recipe? PickRecipe(RecipeCategoryKind category, MealDietPreference diet, int seed)
    {
        var list = RecipeCatalog.GetRecipes(category, diet);
        if (list.Count == 0)
            return null;
        var idx = Math.Abs(seed) % list.Count;
        return list[idx];
    }

    private static int GoalCalories(FitnessGoalType goal, double weightKg, double bmi)
    {
        var maintenance = (int)Math.Round(weightKg * 30);
        var delta = goal switch
        {
            FitnessGoalType.LoseWeight => -450,
            FitnessGoalType.GainWeight => +350,
            FitnessGoalType.BuildMuscle => +250,
            FitnessGoalType.ImproveEndurance => +150,
            FitnessGoalType.Recomposition => 0,
            FitnessGoalType.GetFit => -100,
            _ => 0
        };

        if (bmi >= 30 && goal != FitnessGoalType.GainWeight)
            delta -= 100;
        if (bmi < 18.5 && goal != FitnessGoalType.LoseWeight)
            delta += 150;

        var target = maintenance + delta;
        target = Math.Clamp(target, 1400, 3600);
        return ((target + 25) / 50) * 50;
    }

    private static string DailyFocus(FitnessGoalType goal, int day)
    {
        var week = day switch
        {
            <= 7 => 1,
            <= 14 => 2,
            _ => 3
        };

        return goal switch
        {
            FitnessGoalType.LoseWeight => week switch
            {
                1 => "Portion control + daily walking routine",
                2 => "Protein consistency + resistance training",
                _ => "Sustain deficit, improve sleep quality"
            },
            FitnessGoalType.GainWeight => week switch
            {
                1 => "Add calorie-dense healthy meals",
                2 => "Increase meal frequency + progressive lifts",
                _ => "Refine surplus and monitor digestion"
            },
            FitnessGoalType.BuildMuscle => week switch
            {
                1 => "Protein timing and training form",
                2 => "Progressive overload with recovery",
                _ => "Volume progression + mobility work"
            },
            FitnessGoalType.ImproveEndurance => week switch
            {
                1 => "Aerobic base and pacing",
                2 => "Tempo intervals + recovery nutrition",
                _ => "Long effort support + hydration"
            },
            FitnessGoalType.Recomposition => week switch
            {
                1 => "High-protein balanced plate setup",
                2 => "Strength consistency + step count",
                _ => "Body measurements and consistency"
            },
            FitnessGoalType.GetFit => week switch
            {
                1 => "Build routine",
                2 => "Improve movement quality",
                _ => "Consistency and recovery"
            },
            _ => week switch
            {
                1 => "Stable meal timing and hydration",
                2 => "Stress management and active days",
                _ => "Long-term sustainable habits"
            }
        };
    }
}
