namespace MealPannerModifiedApp.Views;

public partial class LandingPage : ContentPage
{
    private CancellationTokenSource? _animCts;
    private bool _themeHandlerReady;
    private bool _fontSizeHandlerReady;

    public LandingPage()
    {
        InitializeComponent();
        ThemePicker.ItemsSource = new[]
        {
            "Lemon Zest (light)",
            "Honey Glow (light)",
            "Cream Vanilla (light)",
            "Obsidian Slate (dark)",
            "Midnight Ocean (dark)",
            "Charcoal Rose (dark)",
            "Forest Night (dark)",
            "Amethyst Dark (dark)",
            "Ember Glow (dark)",
            "Nordic Ice (dark)",
            "Carbon Gold (dark)",
            "Deep Teal (dark)"
        };
        _themeHandlerReady = false;
        ThemePicker.SelectedIndex = (int)ThemeManager.LoadSaved();
        _themeHandlerReady = true;

        FontSizePicker.ItemsSource = FontScaleManager.StepLabels;
        _fontSizeHandlerReady = false;
        FontSizePicker.SelectedIndex = (int)FontScaleManager.LoadSaved();
        _fontSizeHandlerReady = true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _themeHandlerReady = false;
        ThemePicker.SelectedIndex = (int)ThemeManager.LoadSaved();
        _themeHandlerReady = true;

        _fontSizeHandlerReady = false;
        FontSizePicker.SelectedIndex = (int)FontScaleManager.LoadSaved();
        _fontSizeHandlerReady = true;

        _animCts = new CancellationTokenSource();
        _ = RunChefAnimationAsync(_animCts.Token);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _animCts?.Cancel();
        _animCts?.Dispose();
        _animCts = null;
    }

    private async Task RunChefAnimationAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await ChefImage.ScaleTo(1.07, 750, Easing.SinInOut);
            if (ct.IsCancellationRequested) break;
            await ChefImage.ScaleTo(1.0, 750, Easing.SinInOut);
            if (ct.IsCancellationRequested) break;
            await ChefImage.TranslateTo(0, -12, 600, Easing.CubicInOut);
            if (ct.IsCancellationRequested) break;
            await ChefImage.TranslateTo(0, 0, 600, Easing.CubicInOut);
        }
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        if (!_themeHandlerReady || ThemePicker.SelectedIndex < 0)
            return;
        ThemeManager.Apply((MealAppTheme)ThemePicker.SelectedIndex);
    }

    private void OnFontSizeChanged(object? sender, EventArgs e)
    {
        if (!_fontSizeHandlerReady || FontSizePicker.SelectedIndex < 0)
            return;
        FontScaleManager.Apply((AppFontSizeStep)FontSizePicker.SelectedIndex);
    }
}
