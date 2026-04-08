using System.Collections.ObjectModel;
using MealPlannerApp.Models;
using MealPlannerApp.Services;

namespace MealPlannerApp.ViewModels;

public class FoodLibraryViewModel(IFoodLibraryService foodLibraryService) : BaseViewModel
{
    public ObservableCollection<FoodLibraryItem> Foods { get; } = [];
    public Command LoadCommand => new(async () => await LoadFoodsAsync());

    // What this function does:
    // Fetches the visual natural food library dataset.
    // Why it is needed:
    // Helps users discover healthy foods and best consumption timing.
    // Input / Output:
    // Input: None.
    // Output: Populates Foods collection for card rendering.
    public async Task LoadFoodsAsync()
    {
        IsBusy = true;
        Foods.Clear();

        var items = await foodLibraryService.GetFoodsAsync();
        foreach (var item in items)
        {
            Foods.Add(item);
        }

        IsBusy = false;
    }
}
