using Plugin.Maui.Biometric;
using Profitocracy.Core.Integrations;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Resources.Strings;

namespace Profitocracy.Mobile.ViewModels.Auth;

public class AuthPageViewModel : BaseNotifyObject
{
    private readonly ISecurityProvider _securityProvider;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IBiometric _biometricService;

    private int _currentIndex;
    private static readonly Dictionary<string, int> ButtonNumbers = new()
    {
        { "Number1", 1 },
        { "Number2", 2 },
        { "Number3", 3 },
        { "Number4", 4 },
        { "Number5", 5 },
        { "Number6", 6 },
        { "Number7", 7 },
        { "Number8", 8 },
        { "Number9", 9 },
        { "Number0", 0 },
        { "Delete", -1 },
        { "BiometricAuth", -2 },
    };

    public AuthPageViewModel(
        ISecurityProvider securityProvider,
        ISettingsRepository settingsRepository,
        IBiometric biometricService)
    {
        _securityProvider = securityProvider;
        _settingsRepository = settingsRepository;
        _biometricService = biometricService;

        PassCode = [-1, -1, -1, -1];
    }

    public int[] PassCode { get; private set; }

    public async ValueTask<int> ProcessButton(string buttonNumber)
    {
        var numExists = ButtonNumbers.TryGetValue(buttonNumber, out var digit);

        if (!numExists)
        {
            return -1;
        }

        switch (digit)
        {
            case -1:
                RemoveDigit();
                return -1;
            case -2:
                return await ValidateBiometricAsync();
        }

        AddDigit(digit);

        if (_currentIndex == 4)
        {
            return await ValidatePassCodeAsync();
        }

        return -1;
    }

    private void AddDigit(int digit)
    {
        if (_currentIndex == 4)
        {
            return;
        }

        PassCode[_currentIndex] = digit;

        if (_currentIndex < 4)
        {
            _currentIndex++;
        }
    }

    private void RemoveDigit()
    {
        if (_currentIndex == 0 && PassCode[_currentIndex] == -1)
        {
            return;
        }

        PassCode[_currentIndex-1] = -1;

        if (_currentIndex > 0)
        {
            _currentIndex--;
        }
    }

    private async Task<int> ValidatePassCodeAsync()
    {
        Exception? exception = null;

        try
        {
            var settings = await _settingsRepository.GetCurrentSettings();

            // In case when user at this page
            // but there is no active password.
            if (settings?.Authentication.PasswordHash is null
                || string.IsNullOrEmpty(settings.Authentication.PasswordHash))
            {
                return 1;
            }

            var passCode = string.Join("", PassCode.Select(x => x.ToString()));
            var isValid = _securityProvider.ValidatePassword(
                passCode,
                settings.Authentication.PasswordHash);

            if (isValid)
            {
                return 1;
            }
        }
        catch (Exception ex)
        {
            exception = ex;
        }
        finally
        {
            PassCode = [-1, -1, -1, -1];
            _currentIndex = 0;
        }

        if (exception is not null)
        {
            throw exception;
        }

        return 0;
    }

    private async Task<int> ValidateBiometricAsync()
    {
        var settings = await _settingsRepository.GetCurrentSettings();

        if (settings is null)
        {
            return 1;
        }

        if (!settings.Authentication.IsBiometricAuthEnabled)
        {
            return -2;
        }

        var authRequest = new AuthenticationRequest
        {
            AllowPasswordAuth = false,
            AuthStrength = AuthenticatorStrength.Strong,
            Title = AppResources.Auth_Biometric_Title,
            Description = AppResources.Auth_Biometric_Description,
            NegativeText = AppResources.Auth_Biometric_NegativeText,
            Subtitle = AppResources.Auth_Biometric_Subtitle,
        };

        var response = await _biometricService.AuthenticateAsync(authRequest, CancellationToken.None);

        return response.Status switch
        {
            BiometricResponseStatus.Failure => -2,
            BiometricResponseStatus.Success => 1,
            _ => 0,
        };
    }
}
