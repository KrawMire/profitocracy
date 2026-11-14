using System.Globalization;

namespace Profitocracy.Mobile.Converters;

/// <summary>
/// Maps a transaction spending type (0 - main, 1 - secondary, 2 - saved,
/// null or -1 - income) to the colour of the 50/30/20 palette.
/// </summary>
public class SpendingTypeColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var key = value switch
        {
            1 => "SecondaryColor",
            2 => "SavedColor",
            _ => Application.Current?.RequestedTheme == AppTheme.Dark ? "DarkMainColor" : "MainColor"
        };

        return Application.Current?.Resources.TryGetValue(key, out var color) == true
            ? color
            : null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
