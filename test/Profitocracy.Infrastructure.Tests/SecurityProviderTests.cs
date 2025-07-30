using Profitocracy.Infrastructure.Integrations;

namespace Profitocracy.Infrastructure.Tests;

public class SecurityProviderTests
{
    private readonly SecurityProvider _securityProvider;

    public SecurityProviderTests()
    {
        _securityProvider = new SecurityProvider();
    }

    [Fact]
    public void ValidatePassword_WithCorrectPassword_ReturnsTrue()
    {
        var password = "TestPassword123";
        var hashedPassword = _securityProvider.HashPassword(password);

        var result = _securityProvider.ValidatePassword(password, hashedPassword);

        Assert.True(result);
    }

    [Fact]
    public void ValidatePassword_WithIncorrectPassword_ReturnsFalse()
    {
        var password = "TestPassword123";
        var wrongPassword = "WrongPassword123";
        var hashedPassword = _securityProvider.HashPassword(password);

        var result = _securityProvider.ValidatePassword(wrongPassword, hashedPassword);

        Assert.False(result);
    }

    [Fact]
    public void HashPassword_ReturnsValidFormat()
    {
        var password = "TestPassword123";

        var hashedPassword = _securityProvider.HashPassword(password);

        Assert.Contains(":", hashedPassword);
        var parts = hashedPassword.Split(':');

        Assert.Equal(2, parts.Length);
        Assert.NotEmpty(parts[0]);
        Assert.NotEmpty(parts[1]);
    }

    [Fact]
    public void ValidatePassword_WithInvalidFormat_ThrowsArgumentException()
    {
        var password = "TestPassword123";
        var invalidHashedPassword = "InvalidFormat";

        Assert.Throws<ArgumentException>(() =>
            _securityProvider.ValidatePassword(password, invalidHashedPassword));
    }
}
