namespace MealPannerModifiedApp.Views;

public partial class SettingsPage : ContentPage
{
    private bool _fontSizeReady;

    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        FontSizePicker.ItemsSource = FontScaleManager.StepLabels;
        _fontSizeReady = false;
        FontSizePicker.SelectedIndex = (int)FontScaleManager.LoadSaved();
        _fontSizeReady = true;
    }

    private void OnFontSizeChanged(object? sender, EventArgs e)
    {
        if (!_fontSizeReady || FontSizePicker.SelectedIndex < 0)
            return;
        FontScaleManager.Apply((AppFontSizeStep)FontSizePicker.SelectedIndex);
    }
}
