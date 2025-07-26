using Profitocracy.Core.Integrations;
using Profitocracy.Mobile.Abstractions;

namespace Profitocracy.Mobile.ViewModels.Auth;

public class AuthPageViewModel : BaseNotifyObject
{
    private readonly ISecurityProvider _securityProvider;

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

    public AuthPageViewModel(ISecurityProvider securityProvider)
    {
        _securityProvider = securityProvider;
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
                return -1;
        }

        AddDigit(digit);

        if (_currentIndex == 4)
        {
            return await ValidatePassCode();
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

    private async Task<int> ValidatePassCode()
    {
        var passCode = string.Join("", PassCode.Select(x => x.ToString()));
        var isValid = await _securityProvider.ValidatePassword(passCode);

        if (isValid)
        {
            return 1;
        }

        PassCode = [-1, -1, -1, -1];
        _currentIndex = 0;

        return 0;
    }
}
