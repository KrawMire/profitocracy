namespace Profitocracy.Core.Domain.Model.Summaries.ValueObjects;

[Flags]
public enum SummaryCalculationType
{
    None = 0b0000,
    IncludeDaily = 0b0001,
    IncludeWeekly = 0b0010,
}