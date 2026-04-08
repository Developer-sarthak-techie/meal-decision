namespace MealPlannerApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		UserAppTheme = AppTheme.Dark;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var themeService = Helpers.ServiceLocator.GetService<Services.IThemeService>();
		var theme = themeService.GetCurrentThemeAsync().GetAwaiter().GetResult();
		themeService.ApplyThemeAsync(theme).GetAwaiter().GetResult();

		return new Window(new AppShell());
	}
}
