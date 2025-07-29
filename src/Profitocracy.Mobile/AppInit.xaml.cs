using Profitocracy.Core.Domain.Model.Settings;
using Profitocracy.Core.Domain.Model.Settings.ValueObjects;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Constants;
using Profitocracy.Mobile.Services;
using Profitocracy.Mobile.Views.Settings.Pages;
using System.Globalization;

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

        ThemeService.ChangeTheme(theme);

        if (settings is not null)
        {
            LocalizationService.ChangeCurrentLanguage(settings.Language);
            return settings;
        }

        var lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        if (LocalizationService.SupportedLanguages.Contains(lang))
        {
            LocalizationService.ChangeCurrentLanguage(lang);
        }
        else
        {
            lang = LocalizationService.CurrentLanguage;
        }

        var authSettings = settings?.Authentication ?? new AuthenticationSettings
        {
            IsAuthenticationEnabled = false,
            IsBiometricAuthEnabled = false,
            PasswordHash = null,
        };

        settings = new Settings(
            Guid.NewGuid(),
            theme,
            lang,
            authSettings);

        return await _settingsRepository.CreateOrUpdate(settings);
    }
}
