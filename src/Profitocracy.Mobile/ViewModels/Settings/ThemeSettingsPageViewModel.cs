using Profitocracy.Core.Domain.Model.Settings.ValueObjects;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Services;

namespace Profitocracy.Mobile.ViewModels.Settings;

public class ThemeSettingsPageViewModel : BaseNotifyObject
{
    private readonly ISettingsRepository _settingsRepository;

    public ThemeSettingsPageViewModel(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    private bool _isLightTheme;
    private bool _isDarkTheme;
    private bool _isSystemTheme;

    public bool IsLightTheme
    {
        get => _isLightTheme;
        private set => SetProperty(ref _isLightTheme, value);
    }

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        private set => SetProperty(ref _isDarkTheme, value);
    }

    public bool IsSystemTheme
    {
        get => _isSystemTheme;
        private set => SetProperty(ref _isSystemTheme, value);
    }

    public async Task Initialize()
    {
        var settings = await _settingsRepository.GetCurrentSettings();

        if (settings is null)
        {
            throw new Exception(AppResources.CommonError_GetSettings);
        }

        InitializeThemeFlags(settings.Theme);
    }

    public async Task ChangeTheme(Theme theme)
    {
        var settings = await _settingsRepository.GetCurrentSettings();

        if (settings is null)
        {
            throw new Exception(AppResources.CommonError_GetSettings);
        }

        settings.Theme = theme;

        var saveTask = _settingsRepository.CreateOrUpdate(settings);

        InitializeThemeFlags(theme);
        ThemeService.ChangeTheme(theme);

        await saveTask;
    }

    private void InitializeThemeFlags(Theme theme)
    {
        switch (theme)
        {
            case Theme.Light:
                IsLightTheme = true;
                IsDarkTheme = false;
                IsSystemTheme = false;
                break;
            case Theme.Dark:
                IsLightTheme = false;
                IsDarkTheme = true;
                IsSystemTheme = false;
                break;
            case Theme.System:
            default:
                IsLightTheme = false;
                IsDarkTheme = false;
                IsSystemTheme = true;
                break;
        }
    }
}