using Profitocracy.Mobile.Resources.Strings;

namespace Profitocracy.Mobile.Abstractions;

/// <summary>
/// Base type for content page
/// </summary>
public abstract class BaseContentPage : ContentPage
{
    /// <summary>
    /// Wrap an action execution into a try-catch expression with
    /// showing alert on error occurring.
    /// </summary>
    /// <param name="action">Action to execute</param>
    protected async void ProcessAction(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                AppResources.ErrorAlert_Title,
                $"{AppResources.ErrorAlert_Description}: {ex.Message}",
                AppResources.ErrorAlert_Ok);
        }
    }
}