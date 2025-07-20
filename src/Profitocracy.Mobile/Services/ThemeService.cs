using Profitocracy.Core.Domain.Model.Settings.ValueObjects;

namespace Profitocracy.Mobile.Services;

public static class ThemeService
{
    private static readonly Dictionary<int, AppTheme> DomainAppThemes = new();

    static ThemeService()
    {
        DomainAppThemes.Add((int)Theme.Light, AppTheme.Light);
        DomainAppThemes.Add((int)Theme.Dark, AppTheme.Dark);
        DomainAppThemes.Add((int)Theme.System, AppTheme.Unspecified);
    }

    public static void ChangeTheme(Theme theme)
    {
        if (Application.Current?.UserAppTheme is null)
        {
            return;
        }

        Application.Current.UserAppTheme = DomainAppThemes[(int)theme];
    }
}