using System.Reflection;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;

namespace MealPannerModifiedApp.Infrastructure;

/// <summary>
/// Embeds OpenSans TTFs so PdfSharpCore works on Mac Catalyst / iOS / Android without system fonts.
/// </summary>
internal sealed class OpenSansPdfFontResolver : IFontResolver
{
    public const string Family = "OpenSans";

    private const string RegularEmbeddedName = "MealPannerModifiedApp.OpenSans-Regular.ttf";
    private const string SemiBoldEmbeddedName = "MealPannerModifiedApp.OpenSans-Semibold.ttf";

    private const string RegularFace = $"{Family}#Regular";
    private const string SemiBoldFace = $"{Family}#SemiBold";

    public string DefaultFontName => RegularFace;

    private static readonly Lazy<Dictionary<string, byte[]>> FontBytes = new(LoadFonts);

    private static Dictionary<string, byte[]> LoadFonts()
    {
        var assembly = typeof(OpenSansPdfFontResolver).Assembly;
        return new Dictionary<string, byte[]>(StringComparer.Ordinal)
        {
            [RegularFace] = ReadEmbedded(assembly, RegularEmbeddedName),
            [SemiBoldFace] = ReadEmbedded(assembly, SemiBoldEmbeddedName)
        };
    }

    private static byte[] ReadEmbedded(Assembly assembly, string name)
    {
        using var stream = assembly.GetManifestResourceStream(name)
            ?? throw new InvalidOperationException($"Missing embedded font resource: {name}");
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (!string.Equals(familyName, Family, StringComparison.OrdinalIgnoreCase))
            return null;

        // No italic files embedded; approximate bold with Semibold.
        var face = isBold ? SemiBoldFace : RegularFace;
        return new FontResolverInfo(face);
    }

    public byte[]? GetFont(string faceName)
    {
        return FontBytes.Value.TryGetValue(faceName, out var bytes) ? bytes : null;
    }
}
