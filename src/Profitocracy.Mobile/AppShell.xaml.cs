using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Constants;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Views.Settings.Pages;
using Profitocracy.Mobile.Views.Transactions.Pages;

namespace Profitocracy.Mobile;

public partial class AppShell : Shell
{
    /// <summary>
    /// Route of the "+" tab in the middle of the tab bar.
    /// The tab is never shown, it opens the new transaction sheet instead.
    /// </summary>
    public const string AddTransactionRoute = "add-transaction";

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
            await DisplayAlertAsync("Critical error", ex.Message, "OK");
        }
    }

    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        if (args.Target?.Location.OriginalString.Contains(AddTransactionRoute) != true)
        {
            return;
        }

        args.Cancel();
        Dispatcher.Dispatch(OpenNewTransactionPage);
    }

    /// <summary>
    /// Opens the new transaction sheet over the current page.
    /// </summary>
    /// <param name="isIncome">Whether the sheet should start with the income type</param>
    public async void OpenNewTransactionPage(bool isIncome)
    {
        try
        {
            var editPage = Handler?.MauiContext?.Services.GetService<EditTransactionPage>();

            if (editPage is null)
            {
                return;
            }

            if (isIncome)
            {
                editPage.StartWithIncome();
            }

            await Navigation.PushModalAsync(editPage);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                AppResources.ErrorAlert_Title,
                $"{AppResources.ErrorAlert_Description}: {ex.Message}",
                AppResources.ErrorAlert_Ok);
        }
    }

    private void OpenNewTransactionPage() => OpenNewTransactionPage(isIncome: false);

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
