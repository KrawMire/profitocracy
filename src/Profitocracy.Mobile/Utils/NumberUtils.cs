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
        => FormatMoney(num ?? 0, currencySymbol);

    /// <summary>
    /// Formats money the way the design shows it: currency symbol, group
    /// separators, fixed decimals and a typographic minus, e.g. "−€1,474.67".
    /// </summary>
    /// <param name="num">The amount to format.</param>
    /// <param name="currencySymbol">The symbol of the currency. If null, the default currency symbol is used.</param>
    /// <param name="showPlus">Whether a positive amount gets an explicit "+" sign.</param>
    /// <param name="decimals">Number of decimal places.</param>
    /// <returns>Formatted amount of money.</returns>
    public static string FormatMoney(decimal num, string? currencySymbol, bool showPlus = false, int decimals = 2)
    {
        currencySymbol ??= Currency.AvailableCurrencies.DefaultCurrency.Symbol;

        var rounded = Math.Round(num, decimals);
        var absolute = Math.Abs(rounded).ToString($"N{decimals}", CultureInfo.CurrentCulture);
        var sign = rounded switch
        {
            < 0 => "\u2212",
            > 0 when showPlus => "+",
            _ => string.Empty
        };

        return $"{sign}{currencySymbol}{absolute}";
    }

    /// <summary>
    /// Formats money without decimals when the amount is whole, e.g. "€1,000" but "€9.99".
    /// Used for limits and budgets, which are usually round numbers.
    /// </summary>
    /// <param name="num">The amount to format.</param>
    /// <param name="currencySymbol">The symbol of the currency.</param>
    /// <returns>Formatted amount of money.</returns>
    public static string FormatMoneyCompact(decimal num, string? currencySymbol)
        => FormatMoney(num, currencySymbol, decimals: num == Math.Round(num) ? 0 : 2);

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
