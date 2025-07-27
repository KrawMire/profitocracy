using System.Globalization;
using Profitocracy.Core.Domain.Model.Settings;
using Profitocracy.Core.Domain.Model.Settings.ValueObjects;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Constants;
using Profitocracy.Mobile.Services.Static;
using Profitocracy.Mobile.Views.Settings.Pages;

namespace Profitocracy.Mobile;

public class InitEventArgs
{
    public bool RequireAuthentication { get; set; }
}

public partial class AppInit : BaseContentPage
{
    private readonly ISettingsRepository _settingsRepository;

    public AppInit(ISettingsRepository settingsRepository)
    {
        InitializeComponent();

        Routing.RegisterRoute(RoutesConstants.SetupPage, typeof(EditProfilePage));
        _settingsRepository = settingsRepository;
    }

    public event EventHandler<InitEventArgs> Initialized = null!;

    private void AppInit_OnLoaded(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            var args = await InitializeApplication();
            Initialized.Invoke(this, args);
        });
    }

    private async Task<InitEventArgs> InitializeApplication()
    {
        var settings = await InitializeSettings();

        var initArgs = new InitEventArgs
        {
            RequireAuthentication = settings.Authentication.IsAuthenticationEnabled,
        };

        return initArgs;
    }

    private async Task<Settings> InitializeSettings()
    {
        var settings = await _settingsRepository.GetCurrentSettings();
        var theme = settings?.Theme ?? Theme.System;

        settings ??= BuildDefaultSettings(theme);

        await _settingsRepository.CreateOrUpdate(settings);

        await BootstrapAppWithSettings(settings);

        return settings;
    }

    private static Settings BuildDefaultSettings(Theme theme)
    {
        var lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        if (!LocalizationService.SupportedLanguages.Contains(lang))
        {
            lang = LocalizationService.CurrentLanguage;
        }

        var authSettings = new AuthenticationSettings
        {
            IsAuthenticationEnabled = false,
            IsBiometricAuthEnabled = false,
            PasswordHash = null,
        };

        var notificationSettings = new NotificationsSettings
        {
            IsEnabled = false,
            AddTransactionReminder = new NotificationEventSettings
            {
                IsEnabled = false,
                ScheduledTime = TimeSpan.Zero,
            },
        };

        var settings = new Settings(
            Guid.NewGuid(),
            theme,
            lang,
            authSettings,
            notificationSettings);

        return settings;
    }

    private static async Task BootstrapAppWithSettings(Settings settings)
    {
        LocalizationService.ChangeCurrentLanguage(settings.Language);
        ThemeService.ChangeTheme(settings.Theme);

        if (settings.Notifications.IsEnabled)
        {
            await NotificationService.ScheduleAddTransactionReminderNotification(
                settings.Notifications.AddTransactionReminder.ScheduledTime);
        }
    }
}