using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Constants;
using Profitocracy.Mobile.Views.Settings.Pages;

namespace Profitocracy.Mobile;

public partial class AppShell : Shell
{
    private readonly IProfileRepository _profileRepository;

    public AppShell(IProfileRepository profileRepository)
    {
        InitializeComponent();

        Routing.RegisterRoute(RoutesConstants.SetupPage, typeof(EditProfilePage));
        _profileRepository = profileRepository;
    }

    private async void AppShell_OnLoaded(object? sender, EventArgs e)
    {
        try
        {
            await InitializeApplication();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Critical error", ex.Message, "OK");
        }
    }

    private async Task InitializeApplication()
    {
        await CheckProfileExists();
    }

    /// <summary>
    /// Should be called last, because if there is no existing
    /// profile, user will be redirected to the Setup page.
    /// </summary>
    private async Task CheckProfileExists()
    {
        var profile = await _profileRepository.GetCurrentProfileId();

        if (profile is null)
        {
            await Current.GoToAsync(RoutesConstants.SetupPage);
        }
    }
}