using System.Text.RegularExpressions;
using Profitocracy.Core.Integrations;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Resources.Strings;

namespace Profitocracy.Mobile.ViewModels.Auth;

public class AuthSettingsPageViewModel : BaseNotifyObject
{
    private const string RegexString = @"^\d{4}$";

    private bool _isEnabled;
    private bool _isBiometricEnabled;
    private string _code = string.Empty;

    private readonly ISettingsRepository _settingsRepository;
    private readonly ISecurityProvider _securityProvider;

    public AuthSettingsPageViewModel(
        ISettingsRepository settingsRepository,
        ISecurityProvider securityProvider)
    {
        _settingsRepository = settingsRepository;
        _securityProvider = securityProvider;
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    public bool IsBiometricEnabled
    {
        get => _isBiometricEnabled;
        set => SetProperty(ref _isBiometricEnabled, value);
    }

    public string Code
    {
        get => _code;
        set => SetProperty(ref _code, value);
    }

    public async Task Initialize()
    {
        var settings = await _settingsRepository.GetCurrentSettings();

        if (settings is null)
        {
            throw new Exception(AppResources.CommonError_GetSettings);
        }

        IsEnabled = settings.Authentication.IsAuthenticationEnabled;
        IsBiometricEnabled = settings.Authentication.IsBiometricAuthEnabled;
    }

    public async Task Save()
    {
        var settings = await _settingsRepository.GetCurrentSettings();

        if (settings is null)
        {
            throw new Exception(AppResources.CommonError_GetSettings);
        }

        if (IsEnabled)
        {
            var regex = new Regex(RegexString);
            var codeValid = regex.IsMatch(Code);

            if (!codeValid)
            {
                throw new Exception(AppResources.AuthSettings_Error_PassCodeFormat);
            }

            settings.Authentication.IsAuthenticationEnabled = IsEnabled;
            settings.Authentication.IsBiometricAuthEnabled = IsBiometricEnabled;
            settings.Authentication.PasswordHash = _securityProvider.HashPassword(Code);
        }
        else
        {
            settings.Authentication.IsAuthenticationEnabled = IsEnabled;
            settings.Authentication.IsBiometricAuthEnabled = false;
            settings.Authentication.PasswordHash = null;
        }

        await _settingsRepository.CreateOrUpdate(settings);
    }
}
