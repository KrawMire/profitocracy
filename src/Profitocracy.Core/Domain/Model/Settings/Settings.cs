using Profitocracy.Core.Domain.Model.Settings.ValueObjects;
using Profitocracy.Core.Domain.SharedKernel;

namespace Profitocracy.Core.Domain.Model.Settings;

public class Settings : AggregateRoot<Guid>
{
    public Settings(
        Guid id,
        Theme theme,
        string language,
        AuthenticationSettings authSettings) : base(id)
    {
        Theme = theme;
        Language = language;
        Authentication = authSettings;
    }

    /// <summary>
    /// Current used theme.
    /// </summary>
    public Theme Theme { get; set; }

    /// <summary>
    /// Language is represented by a lang code.
    /// (Example: English - en, Russian - ru)
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Authentication-related settings.
    /// </summary>
    public AuthenticationSettings Authentication { get; private set; }

    /// <summary>
    /// Enables authentication by configuring authentication settings.
    /// </summary>
    /// <param name="useBiometricAuth">Specifies whether biometric authentication should be enabled.</param>
    /// <param name="passwordHash">The hashed password to be used for authentication.</param>
    public void EnableAuthentication(bool useBiometricAuth, string passwordHash)
    {
        Authentication = new AuthenticationSettings
        {
            IsAuthenticationEnabled = true,
            IsBiometricAuthEnabled = useBiometricAuth,
            PasswordHash = passwordHash,
        };
    }

    /// <summary>
    /// Disables authentication by resetting authentication settings to default values.
    /// </summary>
    public void DisableAuthentication()
    {
        Authentication = new AuthenticationSettings
        {
            IsAuthenticationEnabled = false,
            IsBiometricAuthEnabled = false,
            PasswordHash = null,
        };
    }
}