using Profitocracy.Core.Domain.SharedKernel;

namespace Profitocracy.Core.Domain.Model.Settings.ValueObjects;

/// <summary>
/// Represents authentication-related settings.
/// </summary>
public class AuthenticationSettings : ValueObject
{
    /// <summary>
    /// Indicates whether authentication is enabled in the application.
    /// </summary>
    public required bool IsAuthenticationEnabled { get; set; }

    /// <summary>
    /// Determines whether biometric authentication is enabled in the app.
    /// </summary>
    public required bool IsBiometricAuthEnabled { get; set; }

    /// <summary>
    /// A hashed authentication code to the app.
    /// </summary>
    public required string? PasswordHash { get; set; }
}
