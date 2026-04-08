using Microsoft.Extensions.DependencyInjection;

namespace MealPlannerApp.Helpers;

public static class ServiceLocator
{
    private static IServiceProvider? _services;

    // What this function does:
    // Stores the app's global service provider instance.
    // Why it is needed:
    // MAUI XAML pages often need parameterless constructors, so this enables safe service resolution in those pages.
    // Input / Output:
    // Input: Built IServiceProvider from MauiProgram.
    // Output: No return value; updates internal static reference.
    public static void Initialize(IServiceProvider services)
    {
        _services = services;
    }

    // What this function does:
    // Resolves a registered dependency of type T from the container.
    // Why it is needed:
    // Keeps view and app bootstrap code clean while still using dependency injection.
    // Input / Output:
    // Input: Generic service type requested by caller.
    // Output: Instance of requested service type.
    public static T GetService<T>() where T : notnull
    {
        if (_services is null)
        {
            throw new InvalidOperationException("Service provider has not been initialized.");
        }

        return _services.GetRequiredService<T>();
    }
}
