using FluentAssertions;
using Profitocracy.Core.Domain.Model.Settings;
using Profitocracy.Core.Domain.Model.Settings.ValueObjects;

namespace Profitocracy.Core.Tests.Domain.Model
{
    public class SettingsTests
    {
        [Fact]
        public void CreateSettings_ShouldInitializeCorrectly()
        {
            var expectedId = Guid.NewGuid();
            const Theme expectedTheme = Theme.Dark;
            const string expectedLanguage = "en-US";
            var expectedAuthSettings = new AuthenticationSettings
            {
                IsAuthenticationEnabled = false,
                IsBiometricAuthEnabled = false,
                PasswordHash = null,
            };

            var settings = new Settings(
                expectedId,
                expectedTheme, expectedLanguage, expectedAuthSettings);

            settings.Id.Should().Be(expectedId);
            settings.Theme.Should().Be(expectedTheme);
            settings.Language.Should().Be(expectedLanguage);
            settings.Authentication.Should().BeEquivalentTo(expectedAuthSettings);
        }

        [Fact]
        public void EnableAuthentication_ShouldBeEnabled()
        {
            var settings = new Settings(
                Guid.NewGuid(),
                Theme.Dark,
                "en-US",
                new AuthenticationSettings
                {
                    IsAuthenticationEnabled = false,
                    IsBiometricAuthEnabled = false,
                    PasswordHash = null,
                });

            settings.EnableAuthentication(true, "hashedPassword");

            settings.Authentication.IsAuthenticationEnabled.Should().BeTrue();
            settings.Authentication.IsBiometricAuthEnabled.Should().BeTrue();
            settings.Authentication.PasswordHash.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void DisableAuthentication_ShouldBeDisabled()
        {
            var settings = new Settings(
                Guid.NewGuid(),
                Theme.Dark,
                "en-US",
                new AuthenticationSettings
                {
                    IsAuthenticationEnabled = false,
                    IsBiometricAuthEnabled = false,
                    PasswordHash = null,
                });

            settings.DisableAuthentication();

            settings.Authentication.IsAuthenticationEnabled.Should().BeFalse();
            settings.Authentication.IsBiometricAuthEnabled.Should().BeFalse();
            settings.Authentication.PasswordHash.Should().BeNull();
        }
    }
}
