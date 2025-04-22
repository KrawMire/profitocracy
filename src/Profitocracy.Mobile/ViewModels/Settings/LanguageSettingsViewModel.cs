using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Services;
using Profitocracy.Mobile.Views.Settings.Pages;

namespace Profitocracy.Mobile.ViewModels.Settings;

public class LanguageOption : BaseNotifyObject
{
    private bool _isSelected;

    public string Code { get; set; }
    public string DisplayName { get; set; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

public class LanguageSettingsViewModel : BaseNotifyObject
{
    private readonly ISettingsRepository _settingsRepository;
    private ObservableCollection<LanguageOption> _availableLanguages;

    public LanguageSettingsViewModel(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
        SelectLanguageCommand = new Command<string>(async (language) => await OnLanguageSelected(language));
        _availableLanguages = new ObservableCollection<LanguageOption>();
    }

    public ObservableCollection<LanguageOption> AvailableLanguages
    {
        get => _availableLanguages;
        set => SetProperty(ref _availableLanguages, value);
    }

    public ICommand SelectLanguageCommand { get; }

    public async Task Initialize()
    {
        var settings = await _settingsRepository.GetCurrentSettings();

        if (settings is null)
        {
            throw new Exception(AppResources.CommonError_GetSettings);
        }

        InitializeLanguageOptions(settings.Language);
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
        UpdateSelectedLanguage(language);

        await saveTask;
    }

    private async Task OnLanguageSelected(string languageCode)
    {
        await ChangeLanguage(languageCode);

        // Send a message using WeakReferenceMessenger instead of MessagingCenter
        WeakReferenceMessenger.Default.Send(new LanguageChangedMessage());
    }

    private void InitializeLanguageOptions(string currentLanguage)
    {
        AvailableLanguages.Clear();

        // Add supported languages dynamically from LocalizationService
        foreach (var lang in LocalizationService.SupportedLanguages)
        {
            var displayName = GetLanguageDisplayName(lang);

            AvailableLanguages.Add(new LanguageOption
            {
                Code = lang,
                DisplayName = displayName,
                IsSelected = lang == currentLanguage
            });
        }
    }

    private string GetLanguageDisplayName(string languageCode)
    {
        return languageCode switch
        {
            LocalizationService.English => AppResources.Languages_English,
            LocalizationService.Russian => AppResources.Languages_Russian,
            LocalizationService.French => AppResources.Languages_French,
            LocalizationService.Spanish => AppResources.Languages_Spanish,
            LocalizationService.CyrillicSerbian => AppResources.Languages_SerbianCyrillic,
            LocalizationService.LatinSerbian => AppResources.Languages_SerbianLatin,
            _ => languageCode
        };
    }

    private void UpdateSelectedLanguage(string selectedLanguage)
    {
        foreach (var language in AvailableLanguages)
        {
            language.IsSelected = language.Code == selectedLanguage;
        }
    }
}
