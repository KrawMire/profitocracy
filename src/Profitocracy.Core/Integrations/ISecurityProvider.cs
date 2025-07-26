namespace Profitocracy.Core.Integrations;

/// <summary>
/// Provides methods for handling security-related operations, such as
/// password validation and hashing.
/// </summary>
public interface ISecurityProvider
{
    /// <summary>
    /// Validates whether the provided password matches the expected password.
    /// </summary>
    /// <param name="password">The input password to validate.</param>
    /// <param name="expectedPassword">The expected password to compare against.</param>
    /// <returns>Boolean value indicating whether the password validation was successful.</returns>
    bool ValidatePassword(string password, string expectedPassword);

    /// <summary>
    /// Generates a secure hash from the provided password.
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <returns>String representing the hashed password.</returns>
    string HashPassword(string password);
}
