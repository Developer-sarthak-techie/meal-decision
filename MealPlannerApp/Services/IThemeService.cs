using MealPlannerApp.Models;

namespace MealPlannerApp.Services;

public interface IThemeService
{
    Task<AppThemeVariant> GetCurrentThemeAsync();
    Task ApplyThemeAsync(AppThemeVariant variant);
}
