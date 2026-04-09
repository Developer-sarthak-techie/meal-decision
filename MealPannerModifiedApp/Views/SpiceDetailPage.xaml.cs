using MealPannerModifiedApp.Data;

namespace MealPannerModifiedApp.Views;

public partial class SpiceDetailPage : ContentPage, IQueryAttributable
{
    public SpiceDetailPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("SpiceId", out var raw) || raw?.ToString() is not string id)
            return;
        LoadSpice(Uri.UnescapeDataString(id));
    }

    private void LoadSpice(string id)
    {
        var s = SpiceCatalog.FindById(id);
        if (s is null)
        {
            TitleLabel.Text = "Entry not found";
            return;
        }

        Title = s.Name.Length > 40 ? string.Concat(s.Name.AsSpan(0, 40), "…") : s.Name;
        TitleLabel.Text = s.Name;
        CategoryLabel.Text = s.Category;
        EvidenceLabel.Text = s.EvidenceNote;
        Fill(BenefitsBlock, s.Benefits);
        Fill(UsesBlock, s.HealthUses);
    }

    private static void Fill(VerticalStackLayout stack, IReadOnlyList<string> lines)
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
}
