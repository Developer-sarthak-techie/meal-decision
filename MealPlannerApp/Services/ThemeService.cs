using MealPlannerApp.Models;

namespace MealPlannerApp.Services;

public class ThemeService(IStorageService storageService) : IThemeService
{
    private static void SetThemeColor(ResourceDictionary root, string key, string hex)
    {
        var color = Color.FromArgb(hex);
        root[key] = color;
        foreach (var merged in root.MergedDictionaries)
        {
            if (merged.ContainsKey(key))
                merged[key] = color;
        }
    }

    private const string ThemeKey = "app_theme_variant";

    // What this function does:
    // Returns the currently selected app theme variant.
    // Why it is needed:
    // Settings page needs current theme state to render and persist user preference.
    // Input / Output:
    // Input: None.
    // Output: Active AppThemeVariant value.
    public async Task<AppThemeVariant> GetCurrentThemeAsync()
    {
        var stored = await storageService.GetAsync<AppThemeVariant?>(ThemeKey);
        return stored ?? AppThemeVariant.Ocean;
    }

    // What this function does:
    // Applies a full color palette to the app resource dictionary and persists choice.
    // Why it is needed:
    // Enables premium customer themes (Ocean, Crimson, Sunset) requested in product UI.
    // Input / Output:
    // Input: Selected AppThemeVariant.
    // Output: Side-effect updates to app colors and persisted theme.
    public async Task ApplyThemeAsync(AppThemeVariant variant)
    {
        var palette = variant switch
        {
            AppThemeVariant.Crimson => new ThemePalette
            {
                Primary = "#F43F5E",
                Secondary = "#FB7185",
                Background = "#12070B",
                Surface = "#1B0C13",
                CardSurface = "#2A111C",
                TextPrimary = "#FFE4E6",
                TextSecondary = "#FDBAC6",
                PanelBorder = "#7F1D34",
                AccentSoft = "#3D1121",
                GradientStart = "#F43F5E",
                GradientEnd = "#B91C1C"
            },
            AppThemeVariant.Sunset => new ThemePalette
            {
                Primary = "#FB923C",
                Secondary = "#FDBA74",
                Background = "#1A1208",
                Surface = "#2A1B0C",
                CardSurface = "#3A250F",
                TextPrimary = "#FFF7ED",
                TextSecondary = "#FCD9AA",
                PanelBorder = "#A16207",
                AccentSoft = "#4A2E0F",
                GradientStart = "#FB923C",
                GradientEnd = "#F97316"
            },
            _ => new ThemePalette
            {
                Primary = "#22D3EE",
                Secondary = "#67E8F9",
                Background = "#060A13",
                Surface = "#0F172A",
                CardSurface = "#112038",
                TextPrimary = "#F8FAFC",
                TextSecondary = "#94A3B8",
                PanelBorder = "#2A3A5B",
                AccentSoft = "#1E3751",
                GradientStart = "#22D3EE",
                GradientEnd = "#3B82F6"
            }
        };

        var resources = Application.Current?.Resources;
        if (resources is not null)
        {
            SetThemeColor(resources, "Primary", palette.Primary);
            SetThemeColor(resources, "Secondary", palette.Secondary);
            SetThemeColor(resources, "Background", palette.Background);
            SetThemeColor(resources, "Surface", palette.Surface);
            SetThemeColor(resources, "CardSurface", palette.CardSurface);
            SetThemeColor(resources, "TextPrimary", palette.TextPrimary);
            SetThemeColor(resources, "TextSecondary", palette.TextSecondary);
            SetThemeColor(resources, "PanelBorder", palette.PanelBorder);
            SetThemeColor(resources, "AccentSoft", palette.AccentSoft);
            SetThemeColor(resources, "GradientStart", palette.GradientStart);
            SetThemeColor(resources, "GradientEnd", palette.GradientEnd);
        }

        var shell = Shell.Current;
        if (shell is not null)
        {
            var bg = Color.FromArgb(palette.Background);
            var surface = Color.FromArgb(palette.Surface);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                shell.BackgroundColor = bg;
                shell.FlyoutBackgroundColor = surface;
            });
        }

        await storageService.SaveAsync(ThemeKey, variant);
    }

    private class ThemePalette
    {
        public string Primary { get; set; } = string.Empty;
        public string Secondary { get; set; } = string.Empty;
        public string Background { get; set; } = string.Empty;
        public string Surface { get; set; } = string.Empty;
        public string CardSurface { get; set; } = string.Empty;
        public string TextPrimary { get; set; } = string.Empty;
        public string TextSecondary { get; set; } = string.Empty;
        public string PanelBorder { get; set; } = string.Empty;
        public string AccentSoft { get; set; } = string.Empty;
        public string GradientStart { get; set; } = string.Empty;
        public string GradientEnd { get; set; } = string.Empty;
    }
}
