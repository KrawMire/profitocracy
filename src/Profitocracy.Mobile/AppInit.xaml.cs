using System.Globalization;
using Profitocracy.Core.Domain.Model.Settings;
using Profitocracy.Core.Domain.Model.Settings.ValueObjects;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Constants;
using Profitocracy.Mobile.Services;
using Profitocracy.Mobile.Views.Settings.Pages;

namespace Profitocracy.Mobile;

public partial class AppInit : BaseContentPage
{
    private readonly ISettingsRepository _settingsRepository;

    public AppInit(ISettingsRepository settingsRepository)
    {
        InitializeComponent();

        Routing.RegisterRoute(RoutesConstants.SetupPage, typeof(EditProfilePage));
        _settingsRepository = settingsRepository;
    }

    public event EventHandler<EventArgs> Initialized = null!;

    private void AppInit_OnLoaded(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await InitializeApplication();
            Initialized.Invoke(this, e);
        });
    }

    private async Task InitializeApplication()
    {
        await InitializeSettings();
    }

    private async Task InitializeSettings()
    {
        var settings = await _settingsRepository.GetCurrentSettings();
        var theme = settings?.Theme ?? Theme.System;

        ThemeService.ChangeTheme(theme);

        if (settings is not null)
        {
            LocalizationService.ChangeCurrentLanguage(settings.Language);
            return;
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

        settings = new Settings(
            Guid.NewGuid(),
            theme,
            lang);

        await _settingsRepository.CreateOrUpdate(settings);
    }
}