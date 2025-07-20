using Profitocracy.Core.Domain.Model.Settings.ValueObjects;
using Profitocracy.Core.Domain.SharedKernel;

namespace Profitocracy.Core.Domain.Model.Settings;

public class Settings : AggregateRoot<Guid>
{
    public Settings(
        Guid id,
        Theme theme,
        string language) : base(id)
    {
        Theme = theme;
        Language = language;
    }

    /// <summary>
    /// Current used theme.
    /// </summary>
    public Theme Theme { get; set; }

    /// <summary>
    /// Language is represented by lang code.
    /// (Example: English - en, Russian - ru)
    /// </summary>
    public string Language { get; set; }
}