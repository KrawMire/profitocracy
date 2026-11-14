using System.Globalization;

namespace Profitocracy.Mobile.Converters;

/// <summary>
/// Converts an actual/planned ratio into a flag that tells
/// whether the planned amount has been exceeded.
/// </summary>
public class OverLimitConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            float ratio => ratio > 1,
            double ratio => ratio > 1,
            _ => false
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
