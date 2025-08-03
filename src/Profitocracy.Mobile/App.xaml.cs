using Profitocracy.Mobile.Views.Auth;

namespace Profitocracy.Mobile;

public partial class App : Application
{
    private readonly AppInit _appInit;
    private readonly IServiceProvider _serviceProvider;

    public App(AppInit appInit, IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _appInit = appInit;
        _serviceProvider = serviceProvider;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var newWindow = new Window(_appInit);

        _appInit.Initialized += (_, args) =>
        {
            if (args.RequireAuthentication)
            {
                var authPage = _serviceProvider.GetRequiredService<AuthPage>();

                authPage.AuthPassed += (_, _) =>
                {
                    var page = _serviceProvider.GetRequiredService<AppShell>();
                    newWindow.Page = page;
                };

                newWindow.Page = authPage;
                return;
            }

            var page = _serviceProvider.GetRequiredService<AppShell>();
            newWindow.Page = page;
        };

        return newWindow;
    }
}
