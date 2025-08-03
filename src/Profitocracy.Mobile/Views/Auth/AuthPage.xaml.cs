using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.ViewModels.Auth;

namespace Profitocracy.Mobile.Views.Auth;

public partial class AuthPage : BaseContentPage
{
    private readonly AuthPageViewModel _viewModel;

    private readonly Style? _missedCodeDigitStyle;
    private readonly Style? _filledCodeDigitStyle;

    public AuthPage(AuthPageViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        HandleDigits();

        if (Application.Current is null)
        {
            return;
        }

        if (Application.Current.Resources.TryGetValue("MissedAuthCodeDigit", out var missedStyle))
        {
            _missedCodeDigitStyle = (Style)missedStyle;
        }

        if (Application.Current.Resources.TryGetValue("ActiveAuthCodeDigit", out var activeStyle))
        {
            _filledCodeDigitStyle = (Style)activeStyle;
        }
    }

    public event EventHandler<EventArgs> AuthPassed = null!;

    private void AuthCodeInputButton_Clicked(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            if (sender is not Border inputBorder)
            {
                return;
            }

            var name = inputBorder.AutomationId;
            var result = await _viewModel.ProcessButton(name);

            HandleDigits();

            switch (result)
            {
                case -2:
                    await DisplayAlert(
                        AppResources.Auth_InvalidBiometricAlert_Title,
                        AppResources.Auth_InvalidBiometricAlert_Description,
                        AppResources.Auth_InvalidBiometricAlert_Ok);
                    break;
                case -1:
                    return;
                case 0:
                    await DisplayAlert(
                        AppResources.Auth_InvalidPasswordAlert_Title,
                        AppResources.Auth_InvalidPasswordAlert_Description,
                        AppResources.Auth_InvalidPasswordAlert_Ok);
                    break;
                case 1:
                    AuthPassed.Invoke(this, EventArgs.Empty);
                    break;
            }
        });
    }

    private void HandleDigits()
    {
        var code = _viewModel.PassCode;

        LabelCode1.Text = code[0] == -1 ? "" : "\uf314";
        LabelCode2.Text = code[1] == -1 ? "" : "\uf314";
        LabelCode3.Text = code[2] == -1 ? "" : "\uf314";
        LabelCode4.Text = code[3] == -1 ? "" : "\uf314";

        if (_missedCodeDigitStyle is not null && _filledCodeDigitStyle is not null)
        {
            BorderCode1.Style = code[0] == -1
                ? _missedCodeDigitStyle
                : _filledCodeDigitStyle;

            BorderCode2.Style = code[1] == -1
                ? _missedCodeDigitStyle
                : _filledCodeDigitStyle;

            BorderCode3.Style = code[2] == -1
                ? _missedCodeDigitStyle
                : _filledCodeDigitStyle;

            BorderCode4.Style = code[3] == -1
                ? _missedCodeDigitStyle
                : _filledCodeDigitStyle;
        }
    }
}
