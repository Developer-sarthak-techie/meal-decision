namespace MealPannerModifiedApp.Views;

public partial class SubscriptionPage : ContentPage
{
    private const string SubscriptionPreferenceKey = "subscription_plan";
    private string _selectedPlan = "Starter (Free)";

    public SubscriptionPage()
    {
        InitializeComponent();
        LoadSavedSelection();
    }

    private void LoadSavedSelection()
    {
        var saved = Preferences.Default.Get(SubscriptionPreferenceKey, "Starter (Free)");
        _selectedPlan = saved;

        FreePlanRadio.IsChecked = saved == "Starter (Free)";
        PlusPlanRadio.IsChecked = saved == "Plus Monthly";
        ProPlanRadio.IsChecked = saved == "Pro Annual";
        FamilyPlanRadio.IsChecked = saved == "Family Plan";

        SelectedPlanLabel.Text = $"Selected plan: {_selectedPlan}";
        StatusLabel.Text = string.Empty;
    }

    private void OnPlanCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        _selectedPlan = sender switch
        {
            RadioButton rb when rb == FreePlanRadio => "Starter (Free)",
            RadioButton rb when rb == PlusPlanRadio => "Plus Monthly",
            RadioButton rb when rb == ProPlanRadio => "Pro Annual",
            RadioButton rb when rb == FamilyPlanRadio => "Family Plan",
            _ => _selectedPlan
        };

        SelectedPlanLabel.Text = $"Selected plan: {_selectedPlan}";
        StatusLabel.Text = string.Empty;
    }

    private void OnConfirmClicked(object? sender, EventArgs e)
    {
        Preferences.Default.Set(SubscriptionPreferenceKey, _selectedPlan);
        StatusLabel.Text = $"Subscription selected: {_selectedPlan}.";
    }
}
