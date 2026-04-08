using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MealPlannerApp.Helpers;
using MealPlannerApp.Services;
using MealPlannerApp.ViewModels;

namespace MealPlannerApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// What this function does:
		// Registers application dependencies for MVVM services and view models.
		// Why it is needed:
		// Keeps architecture testable, maintainable, and scalable as more health intelligence is added.
		// Input / Output:
		// Input: MAUI app builder service collection.
		// Output: Service registrations available through dependency injection container.
		builder.Services.AddSingleton<IStorageService, PreferencesStorageService>();
		builder.Services.AddSingleton<IThemeService, ThemeService>();
		builder.Services.AddSingleton<IUserProfileService, UserProfileService>();
		builder.Services.AddSingleton<IMealEngineService, MealEngineService>();
		builder.Services.AddSingleton<IMealDecisionService, MealDecisionService>();
		builder.Services.AddSingleton<IFitnessService, FitnessService>();
		builder.Services.AddSingleton<IFoodLibraryService, FoodLibraryService>();
		builder.Services.AddSingleton<IJourneyService, JourneyService>();

		builder.Services.AddTransient<OnboardingViewModel>();
		builder.Services.AddTransient<AajKyaBanayeViewModel>();
		builder.Services.AddTransient<DashboardViewModel>();
		builder.Services.AddTransient<DietPlanViewModel>();
		builder.Services.AddTransient<ExerciseViewModel>();
		builder.Services.AddTransient<FoodLibraryViewModel>();
		builder.Services.AddTransient<JourneyViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();
		ServiceLocator.Initialize(app.Services);
		return app;
	}
}
