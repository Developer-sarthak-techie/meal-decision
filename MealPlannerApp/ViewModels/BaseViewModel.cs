using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MealPlannerApp.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    private bool _isBusy;
    private string _title = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    // What this function does:
    // Updates field value and notifies UI bindings only when value has changed.
    // Why it is needed:
    // Avoids duplicate UI refresh and forms the foundation of MVVM data binding.
    // Input / Output:
    // Input: Backing field reference, new value, and optional property name.
    // Output: True if value changed; false otherwise.
    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
        {
            return false;
        }

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    // What this function does:
    // Raises property changed notification to any bound controls.
    // Why it is needed:
    // Keeps UI and ViewModel state synchronized in real time.
    // Input / Output:
    // Input: Property name.
    // Output: Event dispatch side-effect; no return value.
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
