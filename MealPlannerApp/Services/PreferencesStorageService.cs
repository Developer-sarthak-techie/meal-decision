using System.Text.Json;

namespace MealPlannerApp.Services;

public class PreferencesStorageService : IStorageService
{
    // What this function does:
    // Saves any serializable object in local device preferences as JSON.
    // Why it is needed:
    // Provides simple offline persistence for MVP without requiring backend or database setup.
    // Input / Output:
    // Input: Storage key and typed value.
    // Output: Async completion task once persisted.
    public Task SaveAsync<T>(string key, T value)
    {
        var payload = JsonSerializer.Serialize(value);
        Preferences.Default.Set(key, payload);
        return Task.CompletedTask;
    }

    // What this function does:
    // Reads and deserializes previously saved JSON data from preferences.
    // Why it is needed:
    // Restores profile, journey progress, and settings between app launches.
    // Input / Output:
    // Input: Storage key and target type parameter.
    // Output: Deserialized typed data or null if missing/invalid.
    public Task<T?> GetAsync<T>(string key)
    {
        if (!Preferences.Default.ContainsKey(key))
        {
            return Task.FromResult<T?>(default);
        }

        var payload = Preferences.Default.Get(key, string.Empty);
        if (string.IsNullOrWhiteSpace(payload))
        {
            return Task.FromResult<T?>(default);
        }

        var value = JsonSerializer.Deserialize<T>(payload);
        return Task.FromResult(value);
    }

    // What this function does:
    // Deletes a value from local preference storage.
    // Why it is needed:
    // Supports reset journey and profile reset actions from settings.
    // Input / Output:
    // Input: Storage key.
    // Output: Async completion task after deletion.
    public Task RemoveAsync(string key)
    {
        Preferences.Default.Remove(key);
        return Task.CompletedTask;
    }
}
