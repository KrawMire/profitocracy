using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Services;

namespace Profitocracy.Mobile.ViewModels.Settings;

public class LanguageSettingsViewModel : BaseNotifyObject
{
    private readonly ISettingsRepository _settingsRepository;

    public LanguageSettingsViewModel(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }
    
    private bool _isEnglish;
    private bool _isRussian;
    private bool _isFrench;


    public bool IsEnglish
    {
        get => _isEnglish;
        set => SetProperty(ref _isEnglish, value);
    }

    public bool IsRussian
    {
        get => _isRussian;
        set => SetProperty(ref _isRussian, value);
    }
    
    public bool IsFrench
    {
        get => _isFrench;
        set => SetProperty(ref _isFrench, value);
    }

    public async Task Initialize()
    {
        var settings = await _settingsRepository.GetCurrentSettings();

        if (settings is null)
        {
            throw new Exception(AppResources.CommonError_GetSettings);
        }
        
        InitializeThemeFlags(settings.Language);
    }
    
    public async Task ChangeLanguage(string language)
    {
        var settings = await _settingsRepository.GetCurrentSettings();

        if (settings is null)
        {
            throw new Exception(AppResources.CommonError_GetSettings);
        }
        
        settings.Language = language;
        
        var saveTask = _settingsRepository.CreateOrUpdate(settings);
        
        LocalizationService.ChangeCurrentLanguage(language);
        InitializeThemeFlags(language);
        
        await saveTask;
    }
    
    private void InitializeThemeFlags(string language)
    {
        switch (language)
        {
            case "ru":
                IsRussian = true;
                IsEnglish = false;
                IsFrench = false;
                break;
            case "en":
                IsRussian = false;
                IsEnglish = true;
                IsFrench = false;
                break;
            case "fr":
                IsRussian = false;
                IsEnglish = false;
                IsFrench = true;
                break;
        }
    }
}