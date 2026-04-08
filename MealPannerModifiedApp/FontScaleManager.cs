namespace MealPannerModifiedApp;

/// <summary>Discrete text size steps; scales typography app-wide via dynamic resources.</summary>
public enum AppFontSizeStep
{
    Compact,
    Standard,
    Comfortable,
    Large,
    ExtraLarge
}

public static class FontScaleManager
{
    private const string Key = "meal_app_font_step";

    /// <summary>Multiplier applied to base point sizes (UI and PDF export).</summary>
    public static double GetScaleFactor(AppFontSizeStep step) => step switch
    {
        AppFontSizeStep.Compact => 0.85,
        AppFontSizeStep.Standard => 1.0,
        AppFontSizeStep.Comfortable => 1.08,
        AppFontSizeStep.Large => 1.18,
        AppFontSizeStep.ExtraLarge => 1.30,
        _ => 1.0
    };

    public static AppFontSizeStep LoadSaved()
    {
        var v = Preferences.Default.Get(Key, (int)AppFontSizeStep.Standard);
        if (Enum.IsDefined(typeof(AppFontSizeStep), v))
            return (AppFontSizeStep)v;
        return AppFontSizeStep.Standard;
    }

    public static void Apply(AppFontSizeStep step)
    {
        var root = Application.Current?.Resources;
        if (root is null)
            return;

        var mul = GetScaleFactor(step);
        SetDouble(root, "FontSizeMicro", ScalePts(mul, 12));
        SetDouble(root, "FontSizeCaption", ScalePts(mul, 13));
        SetDouble(root, "FontSizeBody", ScalePts(mul, 14));
        SetDouble(root, "FontSizeBodyProminent", ScalePts(mul, 15));
        SetDouble(root, "FontSizeSubtitle", ScalePts(mul, 16));
        SetDouble(root, "FontSizeLead", ScalePts(mul, 18));
        SetDouble(root, "FontSizeListTitle", ScalePts(mul, 20));
        SetDouble(root, "FontSizePageTitle", ScalePts(mul, 22));
        SetDouble(root, "FontSizeSection", ScalePts(mul, 24));
        SetDouble(root, "FontSizeScreenHeader", ScalePts(mul, 26));
        SetDouble(root, "FontSizeHero", ScalePts(mul, 28));
        SetDouble(root, "FontSizeHeadline", ScalePts(mul, 32));
        SetDouble(root, "FontSizeSubHeadline", ScalePts(mul, 24));

        Preferences.Default.Set(Key, (int)step);
    }

    private static double ScalePts(double scale, double basePts) =>
        Math.Max(10, Math.Round(basePts * scale, 0));

    public static double GetPdfScaleMultiplier() => GetScaleFactor(LoadSaved());

    public static string[] StepLabels { get; } =
    [
        "Compact",
        "Standard",
        "Comfortable",
        "Large",
        "Extra large"
    ];

    private static void SetDouble(ResourceDictionary dict, string key, double value)
    {
        dict[key] = value;
        foreach (var merged in dict.MergedDictionaries)
        {
            if (merged.ContainsKey(key))
                merged[key] = value;
        }
    }
}
