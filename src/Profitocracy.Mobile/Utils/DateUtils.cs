using Profitocracy.Mobile.Resources.Strings;
using System.Globalization;

namespace Profitocracy.Mobile.Utils;

/// <summary>
/// Contains common methods for displaying dates and periods
/// </summary>
public static class DateUtils
{
    /// <summary>
    /// Checks whether the period covers exactly one calendar month.
    /// </summary>
    public static bool IsCalendarMonth(DateTime dateFrom, DateTime dateTo)
        => dateFrom.Day == 1
           && dateFrom.Year == dateTo.Year
           && dateFrom.Month == dateTo.Month
           && dateTo.Day == DateTime.DaysInMonth(dateTo.Year, dateTo.Month);

    /// <summary>
    /// Title of a period, e.g. "March 2025" for a calendar month or "05.03 – 04.04" otherwise.
    /// </summary>
    public static string PeriodTitle(DateTime dateFrom, DateTime dateTo)
        => IsCalendarMonth(dateFrom, dateTo)
            ? MonthTitle(dateFrom)
            : $"{dateFrom:dd.MM} – {dateTo:dd.MM}";

    /// <summary>
    /// Month and year, e.g. "March 2025".
    /// </summary>
    public static string MonthTitle(DateTime date)
        => Capitalize(date.ToString("MMMM yyyy", CultureInfo.CurrentCulture));

    /// <summary>
    /// Name of the month, e.g. "March".
    /// </summary>
    public static string MonthName(DateTime date)
        => Capitalize(date.ToString("MMMM", CultureInfo.CurrentCulture));

    /// <summary>
    /// Serial number printed on "the note", e.g. "03·2025 · EUR".
    /// </summary>
    public static string NoteSerial(DateTime date, string currencyCode)
        => $"{date:MM}·{date:yyyy} · {currencyCode}";

    /// <summary>
    /// Short relative day: "Today", "Yesterday" or e.g. "Sat 8 Mar".
    /// </summary>
    public static string ShortDay(DateTime date)
    {
        var today = DateTime.Today;

        if (date.Date == today)
        {
            return AppResources.Common_Today;
        }

        if (date.Date == today.AddDays(-1))
        {
            return AppResources.Common_Yesterday;
        }

        return date.ToString("ddd d MMM", CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Day name of a group of transactions: "Today", "Yesterday" or the day of week.
    /// </summary>
    public static string DayName(DateTime date)
    {
        var today = DateTime.Today;

        if (date.Date == today)
        {
            return AppResources.Common_Today;
        }

        if (date.Date == today.AddDays(-1))
        {
            return AppResources.Common_Yesterday;
        }

        return Capitalize(date.ToString("dddd", CultureInfo.CurrentCulture));
    }

    private static string Capitalize(string text)
        => string.IsNullOrEmpty(text)
            ? text
            : char.ToUpper(text[0], CultureInfo.CurrentCulture) + text[1..];
}
