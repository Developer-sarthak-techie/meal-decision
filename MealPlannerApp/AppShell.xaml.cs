namespace MealPlannerApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
	}

	// What this function does:
	// Auto-navigates new users to onboarding tab when no profile exists.
	// Why it is needed:
	// Keeps the flyout side panel always visible while guiding new users to setup.
	// Input / Output:
	// Input: None.
	// Output: Navigation side-effect to onboarding route if profile is missing.
	protected override async void OnNavigated(ShellNavigatedEventArgs args)
	{
		base.OnNavigated(args);

		if (args.Source == ShellNavigationSource.ShellSectionChanged)
			return;

		var profileService = Helpers.ServiceLocator.GetService<Services.IUserProfileService>();
		var profile = await profileService.GetProfileAsync();
		if (profile is null && !args.Current.Location.ToString().Contains("onboarding"))
		{
			await GoToAsync("//onboarding");
		}
	}
}
