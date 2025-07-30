using SQLite;

namespace Profitocracy.Infrastructure.Persistence.Sqlite.Models.Settings;

internal class SettingsModel
{
    [PrimaryKey]
    public Guid Id { get; set; }

    public int Theme { get; set; }

#pragma warning disable CS8618
    public string Language { get; set; }
    public string? Password { get; set; }
#pragma warning restore CS8618

    public bool IsAuthenticationEnabled { get; set; }
    public bool IsBiometricAuthEnabled { get; set; }
}
