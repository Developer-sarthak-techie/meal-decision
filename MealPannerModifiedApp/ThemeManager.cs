namespace MealPannerModifiedApp;

public enum MealAppTheme
{
    LemonZest,
    HoneyGlow,
    CreamVanilla,
    ObsidianSlate,
    MidnightOcean,
    CharcoalRose,
    ForestNight,
    AmethystDark,
    EmberGlow,
    NordicIce,
    CarbonGold,
    DeepTeal
}

public static class ThemeManager
{
    private const string Key = "meal_app_theme";

    public static MealAppTheme LoadSaved()
    {
        var v = Preferences.Default.Get(Key, (int)MealAppTheme.LemonZest);
        if (Enum.IsDefined(typeof(MealAppTheme), v))
            return (MealAppTheme)v;
        return MealAppTheme.LemonZest;
    }

    public static void Apply(MealAppTheme theme)
    {
        var root = Application.Current?.Resources;
        if (root is null)
            return;

        var p = GetPalette(theme);

        SetColor(root, "PageBackground", p.PageBg);
        SetColor(root, "AccentBright", p.Accent);
        SetColor(root, "AccentSoft", p.AccentSoft);
        SetColor(root, "SurfaceCard", p.Surface);
        SetColor(root, "TextPrimary", p.TextPri);
        SetColor(root, "TextSecondary", p.TextSec);
        SetColor(root, "FlyoutBackground", p.Flyout);
        SetColor(root, "FlyoutHeaderBg", p.FlyHead);
        SetColor(root, "PanelBorder", p.PanelBorder);

        Preferences.Default.Set(Key, (int)theme);

        var shell = Shell.Current;
        if (shell is not null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                shell.FlyoutBackgroundColor = Color.FromArgb(p.Flyout);
                if (Application.Current is not null)
                    Application.Current.UserAppTheme = p.UseDarkAppTheme ? AppTheme.Dark : AppTheme.Light;
            });
        }
        else if (Application.Current is not null)
        {
            Application.Current.UserAppTheme = p.UseDarkAppTheme ? AppTheme.Dark : AppTheme.Light;
        }
    }

    private readonly record struct ThemePalette(
        string PageBg,
        string Accent,
        string AccentSoft,
        string Surface,
        string TextPri,
        string TextSec,
        string Flyout,
        string FlyHead,
        string PanelBorder,
        bool UseDarkAppTheme);

    // Palettes tuned for contrast on Shell, cards, and pickers.
    private static ThemePalette GetPalette(MealAppTheme theme) => theme switch
    {
        MealAppTheme.HoneyGlow => new(
            "#FFF3D6", "#FFB300", "#FFD54F", "#FFFAF0", "#3E2723", "#5D4037",
            "#FFECB3", "#FFC107", "#C2A878", false),

        MealAppTheme.CreamVanilla => new(
            "#FFFEF5", "#FFCA28", "#FFF8E1", "#FFFFFF", "#4E342E", "#6D4C41",
            "#FFFDE7", "#FFE082", "#BCAAA4", false),

        MealAppTheme.ObsidianSlate => new(
            "#0D1117", "#58A6FF", "#21262D", "#161B22", "#F0F6FC", "#8B949E",
            "#010409", "#161B22", "#30363D", true),

        MealAppTheme.MidnightOcean => new(
            "#0A1628", "#22D3EE", "#143352", "#12263F", "#E0F2FE", "#7BA7C2",
            "#071020", "#0F2847", "#1E4976", true),

        MealAppTheme.CharcoalRose => new(
            "#1A1215", "#F472B6", "#3F2A32", "#2D1F24", "#FCE7F3", "#9D8790",
            "#140D10", "#2A1F24", "#5C4550", true),

        MealAppTheme.ForestNight => new(
            "#0F1F14", "#4ADE80", "#234833", "#1A2E1F", "#ECFDF5", "#86A893",
            "#0A150E", "#15281C", "#2D5A40", true),

        MealAppTheme.AmethystDark => new(
            "#1A1025", "#C084FC", "#3D2A52", "#2D1F3D", "#F3E8FF", "#A78BBA",
            "#120818", "#231830", "#524066", true),

        MealAppTheme.EmberGlow => new(
            "#1C1410", "#FB923C", "#422A1C", "#2D2118", "#FFF7ED", "#C4A896",
            "#16100C", "#2D1F15", "#5C4234", true),

        MealAppTheme.NordicIce => new(
            "#111827", "#38BDF8", "#374151", "#1F2937", "#F9FAFB", "#9CA3AF",
            "#0B1220", "#1A2332", "#4B5563", true),

        MealAppTheme.CarbonGold => new(
            "#171717", "#FBBF24", "#404040", "#262626", "#FAFAFA", "#A3A3A3",
            "#0F0F0F", "#262626", "#525252", true),

        MealAppTheme.DeepTeal => new(
            "#042F2E", "#2DD4BF", "#134E4A", "#0F766E", "#CCFBF1", "#5EEAD4",
            "#021C1B", "#115E59", "#0D9488", true),

        _ => /* LemonZest */
            new(
                "#FFF9E3", "#FFC400", "#FFE566", "#FFFBF0", "#3D2914", "#6B5344",
                "#FFF3CD", "#FFE082", "#D4A574", false)
    };

    private static void SetColor(ResourceDictionary root, string key, string hex)
    {
        var c = Color.FromArgb(hex);
        root[key] = c;
        foreach (var merged in root.MergedDictionaries)
        {
            if (merged.ContainsKey(key))
                merged[key] = c;
        }
    }
}
