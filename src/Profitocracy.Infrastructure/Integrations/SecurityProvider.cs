using System.Security.Cryptography;
using Profitocracy.Core.Integrations;

namespace Profitocracy.Infrastructure.Integrations;

public class SecurityProvider : ISecurityProvider
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 1000;

    /// <inheritdoc />
    public bool ValidatePassword(string password, string expectedPassword)
    {
        var parts = expectedPassword.Split(':');

        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid password format", nameof(expectedPassword));
        }

        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);

        var actualHash = HashPassword(password, salt);

        return actualHash.SequenceEqual(expectedHash);
    }

    /// <inheritdoc />
    public string HashPassword(string password)
    {
        var salt = GenerateSalt();
        var hashedPassword = HashPassword(password, salt);

        var formattedPassword = $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hashedPassword)}";
        return formattedPassword;
    }

    private static byte[] HashPassword(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(HashSize);
    }

    private static byte[] GenerateSalt()
    {
        var salt = new byte[SaltSize];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        return salt;
    }
}
