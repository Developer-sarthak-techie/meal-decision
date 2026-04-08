namespace MealPannerModifiedApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		UserAppTheme = AppTheme.Light;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		ThemeManager.Apply(ThemeManager.LoadSaved());
		FontScaleManager.Apply(FontScaleManager.LoadSaved());
		return new Window(new AppShell());
	}
}