using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using System.Globalization;

namespace Profitocracy.Mobile.Utils;

/// <summary>
/// Contains common methods for working with numbers
/// </summary>
public static class NumberUtils
{
    /// <summary>
    /// Round decimal to a specified amount of decimals.
    /// </summary>
    /// <param name="num">Number to round</param>
    /// <param name="decimals">Decimals places</param>
    /// <returns>Rounded decimal if num is not null, otherwise, zero</returns>
    public static decimal RoundDecimal(decimal? num, int decimals = 2)
        => num is null
            ? decimal.Zero
            : Math.Round((decimal)num, decimals);

    /// <summary>
    /// Attempts to parse the provided string as a decimal.
    /// </summary>
    /// <param name="str">The string input to parse.</param>
    /// <param name="result">The parsed decimal value if the operation succeeds; otherwise, zero.</param>
    /// <returns>True if the string was successfully parsed to a decimal; otherwise, false.</returns>
    public static bool TryParseDecimal(string? str, out decimal result)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            result = 0;
            return false;
        }

        str = str.Replace(',', '.');

        return decimal.TryParse(str, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>
    /// Rounds the given decimal to a specified number of decimals and formats it as currency with a symbol.
    /// </summary>
    /// <param name="num">The decimal number to round.</param>
    /// <param name="currencySymbol">The symbol of the currency to prepend to the formatted number. If null, the default currency symbol is used.</param>
    /// <returns>A string representation of the rounded number prefixed with the currency symbol.</returns>
    public static string RoundDecimalMoney(decimal? num, string? currencySymbol)
    {
        currencySymbol ??= Currency.AvailableCurrencies.DefaultCurrency.Symbol;

        return $"{currencySymbol}{RoundDecimal(num)}";
    }

    /// <summary>
    /// Get ratio between two decimal numbers.
    /// Ratio is represented with float value.
    /// </summary>
    /// <param name="val1">First value</param>
    /// <param name="val2">Second value</param>
    /// <returns>Ratio between two decimal numbers</returns>
    public static float GetFloatRatio(decimal val1, decimal val2)
        => (float)(val1 / val2);
}
