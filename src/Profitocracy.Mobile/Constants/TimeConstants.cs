namespace Profitocracy.Mobile.Constants;

public static class TimeConstants
{
    public const int MinHours = 0;
    public const int MinMinutes = 0;
    public const int MinSeconds = 0;
    public const int MinMilliseconds = 0;
    public const int MaxHours = TimeSpan.HoursPerDay - 1;
    public const int MaxMinutes = (int)TimeSpan.MinutesPerHour - 1;
    public const int MaxSeconds = (int)TimeSpan.SecondsPerMinute - 1;
    public const int MaxMilliseconds = (int)TimeSpan.MillisecondsPerSecond - 1;
}
