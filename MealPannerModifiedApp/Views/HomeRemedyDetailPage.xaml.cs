using MealPannerModifiedApp.Data;
using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Views;

public partial class HomeRemedyDetailPage : ContentPage, IQueryAttributable
{
    public HomeRemedyDetailPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("RemedyId", out var raw) || raw?.ToString() is not string id)
            return;
        LoadRemedy(Uri.UnescapeDataString(id));
    }

    private void LoadRemedy(string id)
    {
        var r = HomeRemedyCatalog.FindById(id);
        if (r is null)
        {
            TitleLabel.Text = "Entry not found";
            return;
        }

        Title = r.Title.Length > 42 ? string.Concat(r.Title.AsSpan(0, 42), "…") : r.Title;
        TitleLabel.Text = r.Title;
        CategoryLabel.Text = $"{r.Category} · {r.AudienceLabel}";
        SourceNoteLabel.Text = r.SourceAlignmentNote;

        FillBlock(OverviewBlock, r.Overview);
        FillBlock(SelfCareBlock, r.SelfCare);
        FillBlock(HydrationBlock, r.HydrationAndFood);
        FillBlock(ComfortBlock, r.ComfortMeasures);
        FillBlock(AvoidBlock, r.WhatToAvoid);
        FillBlock(RedFlagsBlock, r.SeeDoctorPromptly);
    }

    private static void FillBlock(VerticalStackLayout stack, IReadOnlyList<string> lines)
    {
        stack.Children.Clear();
        foreach (var line in lines)
        {
            var label = new Label
            {
                Text = "• " + line,
                LineBreakMode = LineBreakMode.WordWrap
            };
            label.SetDynamicResource(Label.FontSizeProperty, "FontSizeBody");
            label.SetDynamicResource(Label.TextColorProperty, "TextSecondary");
            stack.Children.Add(label);
        }
    }

    private void OnMedlinePlus(object? sender, EventArgs e) =>
        _ = Launcher.Default.OpenAsync(new Uri("https://medlineplus.gov/"));

    private void OnNhs(object? sender, EventArgs e) =>
        _ = Launcher.Default.OpenAsync(new Uri("https://www.nhs.uk/conditions/"));
}
