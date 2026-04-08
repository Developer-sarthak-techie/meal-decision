using MealPannerModifiedApp.Views;

namespace MealPannerModifiedApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(RecipeListPage), typeof(RecipeListPage));
		Routing.RegisterRoute(nameof(RecipeDetailPage), typeof(RecipeDetailPage));
		Routing.RegisterRoute(nameof(HomeRemedyDetailPage), typeof(HomeRemedyDetailPage));
	}
}
