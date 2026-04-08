using MealPannerModifiedApp.Models;
using MealPannerModifiedApp;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfStandardFont = PdfSharpCore.Drawing.XFontStyle;

namespace MealPannerModifiedApp.Infrastructure;

internal static class FitnessPlanPdfExporter
{
    private const double MarginPt = 28;
    private const double FooterHeightPt = 34;
    private static readonly object FontResolverLock = new();
    private static bool _fontResolverInstalled;

    public static void Generate(
        string outputPath,
        string generatedAt,
        FitnessGoalType goal,
        MealDietPreference diet,
        double weightKg,
        double heightCm,
        double bmi,
        string bmiBand,
        IReadOnlyList<string> tips,
        IReadOnlyList<FitnessDayPlan> plan,
        string goalLabel,
        string dietLabel)
    {
        EnsurePdfFonts();

        var document = new PdfDocument();
        document.Info.Title = "Fitness plan export";

        var linePen = new XPen(XColors.Gray, 0.5);
        LayoutState state = default;
        NewPage(document, ref state, generatedAt, linePen);

        var pdfScale = FontScaleManager.GetPdfScaleMultiplier();
        var fontSub = new XFont(OpenSansPdfFontResolver.Family, PdfPt(13, pdfScale), PdfStandardFont.Bold);
        var fontBody = new XFont(OpenSansPdfFontResolver.Family, PdfPt(10, pdfScale), PdfStandardFont.Regular);
        var fontSmall = new XFont(OpenSansPdfFontResolver.Family, PdfPt(9, pdfScale), PdfStandardFont.Regular);
        var fontSmallItalic = new XFont(OpenSansPdfFontResolver.Family, PdfPt(9, pdfScale), PdfStandardFont.Italic);
        var fontSmallBold = new XFont(OpenSansPdfFontResolver.Family, PdfPt(9, pdfScale), PdfStandardFont.Bold);

        DrawParagraph(ref state, $"Goal: {goalLabel}", fontSub, XBrushes.Black);
        DrawParagraph(ref state,
            $"Profile: {weightKg:F1} kg, {heightCm:F0} cm, BMI {bmi:F1} ({bmiBand}), Diet: {dietLabel}",
            fontBody, XBrushes.Black);

        state.Y += 6;
        DrawParagraph(ref state, "Health Tips", fontSub, XBrushes.Black);
        foreach (var tip in tips)
            DrawParagraph(ref state, $"- {tip}", fontBody, XBrushes.Black);

        state.Y += 8;
        DrawParagraph(ref state, "21-Day Plan", fontSub, XBrushes.Black);

        foreach (var day in plan)
        {
            var blockLines = new List<string>
            {
                $"Day {day.DayNumber}",
                $"Focus: {day.Focus}",
                $"Breakfast: {day.Breakfast}",
                $"Lunch: {day.Lunch}",
                $"Dinner: {day.Dinner}",
                $"Snack: {day.Snack}",
                $"Target: ~{day.DailyCalories} kcal/day, water {day.WaterMlTarget} ml"
            };

            var boxPadding = 8.0;
            var innerW = state.Right - state.Left - boxPadding * 2;
            var innerH = MeasureBlockHeight(state.Gfx, blockLines, fontBody, fontSmallBold, innerW) + boxPadding * 2;
            EnsureSpace(ref state, document, generatedAt, linePen, innerH + 10);

            var boxTop = state.Y;
            var boxLeft = state.Left;
            var boxW = state.Right - state.Left;
            state.Gfx.DrawRectangle(new XPen(XColors.LightGray, 0.75), boxLeft, boxTop, boxW, innerH);

            var cy = boxTop + boxPadding;
            state.Gfx.DrawString(blockLines[0], fontSmallBold, XBrushes.Black, new XRect(boxLeft + boxPadding, cy, innerW, 14),
                XStringFormats.TopLeft);
            cy += 14;
            for (var i = 1; i < blockLines.Count; i++)
            {
                var h = DrawWrapped(ref state, blockLines[i], fontBody, XBrushes.Black, boxLeft + boxPadding, cy, innerW);
                cy += h;
            }

            state.Y = boxTop + innerH + 10;
        }

        var total = document.PageCount;
        for (var i = 0; i < total; i++)
        {
            var page = document.Pages[i];
            using var footerGfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);
            DrawFooter(footerGfx, page, i + 1, total, linePen, fontSmall, fontSmallItalic);
        }

        document.Save(outputPath);
    }

    private static double PdfPt(double basePt, double scale) =>
        Math.Max(7, Math.Round(basePt * scale, 1));

    private static void EnsurePdfFonts()
    {
        if (_fontResolverInstalled)
            return;
        lock (FontResolverLock)
        {
            if (_fontResolverInstalled)
                return;
            GlobalFontSettings.FontResolver = new OpenSansPdfFontResolver();
            _fontResolverInstalled = true;
        }
    }

    private struct LayoutState
    {
        public XGraphics Gfx;
        public double Y;
        public double Left;
        public double Right;
        public double ContentBottom;
    }

    private static void NewPage(PdfDocument doc, ref LayoutState state, string generatedAt, XPen linePen)
    {
        var page = doc.AddPage();
        state.Gfx = XGraphics.FromPdfPage(page);
        state.Left = MarginPt;
        state.Right = page.Width - MarginPt;
        var footerTop = page.Height - MarginPt - FooterHeightPt;
        state.ContentBottom = footerTop - 6;
        state.Y = MarginPt;

        var scale = FontScaleManager.GetPdfScaleMultiplier();
        var fontHeader = new XFont(OpenSansPdfFontResolver.Family, PdfPt(18, scale), PdfStandardFont.Bold);
        var fontMeta = new XFont(OpenSansPdfFontResolver.Family, PdfPt(9, scale), PdfStandardFont.Regular);
        state.Gfx.DrawString("MealPannerModifiedApp - FitnessGoal", fontHeader, XBrushes.Black,
            new XRect(state.Left, state.Y, state.Right - state.Left, 24), XStringFormats.TopLeft);
        state.Y += 24;
        state.Gfx.DrawString($"Generated: {generatedAt}", fontMeta, new XSolidBrush(XColor.FromArgb(80, 80, 80)),
            new XRect(state.Left, state.Y, state.Right - state.Left, 14), XStringFormats.TopLeft);
        state.Y += 20;
        state.Gfx.DrawLine(linePen, state.Left, state.Y, state.Right, state.Y);
        state.Y += 12;
    }

    private static void EnsureSpace(ref LayoutState state, PdfDocument doc, string generatedAt, XPen linePen, double needed)
    {
        if (state.Y + needed <= state.ContentBottom)
            return;
        state.Gfx.Dispose();
        NewPage(doc, ref state, generatedAt, linePen);
    }

    private static void DrawParagraph(ref LayoutState state, string text, XFont font, XBrush brush)
    {
        var h = DrawWrapped(ref state, text, font, brush, state.Left, state.Y, state.Right - state.Left);
        state.Y += h + 4;
    }

    private static double DrawWrapped(ref LayoutState state, string text, XFont font, XBrush brush, double x, double y, double maxWidth)
    {
        var lines = WrapLines(state.Gfx, text, font, maxWidth);
        var lineHeight = font.Height * 1.15;
        var h = 0.0;
        foreach (var line in lines)
        {
            state.Gfx.DrawString(line, font, brush, new XRect(x, y + h, maxWidth, lineHeight), XStringFormats.TopLeft);
            h += lineHeight;
        }

        return h;
    }

    private static double MeasureBlockHeight(XGraphics gfx, IReadOnlyList<string> lines, XFont body, XFont title, double maxWidth)
    {
        var h = 0.0;
        h += title.Height * 1.15;
        for (var i = 1; i < lines.Count; i++)
            h += WrapLines(gfx, lines[i], body, maxWidth).Count * body.Height * 1.15;
        return h;
    }

    private static List<string> WrapLines(XGraphics gfx, string text, XFont font, double maxWidth)
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(text))
        {
            result.Add("");
            return result;
        }

        foreach (var paragraph in text.Split('\n', StringSplitOptions.None))
        {
            var words = paragraph.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0)
            {
                result.Add("");
                continue;
            }

            var line = words[0];
            for (var i = 1; i < words.Length; i++)
            {
                var candidate = line + " " + words[i];
                if (gfx.MeasureString(candidate, font).Width <= maxWidth)
                    line = candidate;
                else
                {
                    result.Add(line);
                    line = words[i];
                }
            }

            result.Add(line);
        }

        return result;
    }

    private static void DrawFooter(XGraphics gfx, PdfPage page, int pageNumber, int totalPages, XPen linePen, XFont fontPage,
        XFont fontSig)
    {
        var left = MarginPt;
        var right = page.Width - MarginPt;
        var y = page.Height - MarginPt - FooterHeightPt + 8;
        gfx.DrawLine(linePen, left, y, right, y);
        y += 8;
        var pageText = $"Page {pageNumber} / {totalPages}";
        gfx.DrawString(pageText, fontPage, new XSolidBrush(XColor.FromArgb(90, 90, 90)),
            new XRect(left, y, (right - left) / 2, 12), XStringFormats.TopLeft);
        gfx.DrawString("Signature: Sarthak Gupta", fontSig, new XSolidBrush(XColor.FromArgb(100, 100, 100)),
            new XRect(left, y, right - left, 12), XStringFormats.TopRight);
    }
}
