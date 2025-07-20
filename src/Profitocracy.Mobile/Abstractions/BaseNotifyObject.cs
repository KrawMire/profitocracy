using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Profitocracy.Mobile.Abstractions;

/// <summary>
/// Base type for objects which should notify
/// users about any inner state updates
/// </summary>
public abstract class BaseNotifyObject : INotifyPropertyChanged
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Set property value and notify about it
    /// </summary>
    /// <param name="property">Property to change. Must be passed by reference</param>
    /// <param name="value">Value to be set to the property</param>
    /// <param name="propertyName">Property name. Optional, because it is taken automatically</param>
    /// <typeparam name="T">The type of the property</typeparam>
    protected void SetProperty<T>(ref T property, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(property, value))
        {
            return;
        }

        property = value;
        OnPropertyChanged(propertyName);
    }

    /// <summary>
    /// Notify listener about changes of a property
    /// </summary>
    /// <param name="propertyName">Name of the changed property</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}