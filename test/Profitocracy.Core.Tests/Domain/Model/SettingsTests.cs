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
            var expectedNotificationsSettings = new NotificationsSettings
            {
                IsEnabled = false,
                AddTransactionReminder = new NotificationEventSettings
                {
                    IsEnabled = false
                }
            };

            var settings = new Settings(
                expectedId,
                expectedTheme, expectedLanguage, expectedAuthSettings, expectedNotificationsSettings);

            settings.Id.Should().Be(expectedId);
            settings.Theme.Should().Be(expectedTheme);
            settings.Language.Should().Be(expectedLanguage);
            settings.Authentication.Should().BeEquivalentTo(expectedAuthSettings);
            settings.Notifications.Should().BeEquivalentTo(expectedNotificationsSettings);
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
                },
                new NotificationsSettings());

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
                },
                new NotificationsSettings());

            settings.DisableAuthentication();

            settings.Authentication.IsAuthenticationEnabled.Should().BeFalse();
            settings.Authentication.IsBiometricAuthEnabled.Should().BeFalse();
            settings.Authentication.PasswordHash.Should().BeNull();
        }
        
        [Fact]
        public void EnableNotifications_ShouldBeEnabled()
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
                },
                new NotificationsSettings
                {
                    IsEnabled = false,
                    AddTransactionReminder = new NotificationEventSettings
                    {
                        IsEnabled = false
                    }
                });

            settings.EnableNotifications(new NotificationEventSettings
            {
                IsEnabled = true,
                ScheduledTime = TimeSpan.FromDays(1)
            });

            settings.Notifications.IsEnabled.Should().BeTrue();
            settings.Notifications.AddTransactionReminder.Should().NotBeNull();
            settings.Notifications.AddTransactionReminder.IsEnabled.Should().BeTrue();
            settings.Notifications.AddTransactionReminder.ScheduledTime.Should().Be(TimeSpan.FromDays(1));
        }

        [Fact]
        public void DisableNotifications_ShouldBeDisabled()
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
                },
                new NotificationsSettings
                {
                    IsEnabled = false,
                    AddTransactionReminder = new NotificationEventSettings
                    {
                        IsEnabled = false
                    }
                });

            settings.DisableNotifications();

            settings.Notifications.IsEnabled.Should().BeFalse();
            settings.Notifications.AddTransactionReminder.Should().NotBeNull();
            settings.Notifications.AddTransactionReminder.IsEnabled.Should().BeFalse();
            settings.Notifications.AddTransactionReminder.ScheduledTime.Should().Be(TimeSpan.Zero);
        }
    }
}
