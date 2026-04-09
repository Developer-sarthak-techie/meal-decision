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

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _themeHandlerReady = false;
        ThemePicker.SelectedIndex = (int)ThemeManager.LoadSaved();
        _themeHandlerReady = true;

        _fontSizeHandlerReady = false;
        FontSizePicker.SelectedIndex = (int)FontScaleManager.LoadSaved();
        _fontSizeHandlerReady = true;

        DateLineLabel.Text = DateTime.Now.ToString("dddd, MMMM d");

        await RunEntranceAnimationAsync();

        _animCts?.Cancel();
        _animCts?.Dispose();
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

    private async Task RunEntranceAnimationAsync()
    {
        HeaderBlock.Opacity = 0;
        HeaderBlock.TranslationY = 16;
        GlassTray.Opacity = 0;
        GlassTray.TranslationY = 20;
        QuickActionsRow.Opacity = 0;
        QuickActionsRow.TranslationY = 20;
        ChefCard.Opacity = 0;
        ChefCard.TranslationY = 24;
        ChefCard.Scale = 0.96;

        await Task.Delay(40).ConfigureAwait(true);

        await Task.WhenAll(
            HeaderBlock.FadeTo(1, 420, Easing.CubicOut),
            HeaderBlock.TranslateTo(0, 0, 420, Easing.CubicOut));

        await Task.WhenAll(
            GlassTray.FadeTo(1, 380, Easing.CubicOut),
            GlassTray.TranslateTo(0, 0, 380, Easing.CubicOut));

        await Task.WhenAll(
            QuickActionsRow.FadeTo(1, 380, Easing.CubicOut),
            QuickActionsRow.TranslateTo(0, 0, 380, Easing.CubicOut));

        await Task.WhenAll(
            ChefCard.FadeTo(1, 480, Easing.SinOut),
            ChefCard.TranslateTo(0, 0, 480, Easing.CubicOut),
            ChefCard.ScaleTo(1, 480, Easing.CubicOut));
    }

    private async Task RunChefAnimationAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await ChefImage.ScaleTo(1.06, 820, Easing.SinInOut).WaitAsync(ct);
            if (ct.IsCancellationRequested) break;
            await ChefImage.ScaleTo(1.0, 820, Easing.SinInOut).WaitAsync(ct);
            if (ct.IsCancellationRequested) break;
            await ChefImage.TranslateTo(0, -10, 640, Easing.CubicInOut).WaitAsync(ct);
            if (ct.IsCancellationRequested) break;
            await ChefImage.TranslateTo(0, 0, 640, Easing.CubicInOut).WaitAsync(ct);
        }
    }

    private async void OnRecipesTapped(object? sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync("//recipes");
        }
        catch
        {
            /* Route missing or shell not ready — ignore */
        }
    }

    private async void OnSuggestTapped(object? sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync("//suggest");
        }
        catch
        {
        }
    }

    private void OnChefCardTapped(object? sender, EventArgs e)
    {
        if (Shell.Current is not null)
            Shell.Current.FlyoutIsPresented = true;
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
