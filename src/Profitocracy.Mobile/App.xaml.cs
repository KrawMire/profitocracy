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
        _appInit.Initialized += (_, _) => newWindow.Page = _serviceProvider.GetRequiredService<AppShell>();

        return newWindow;
    }
}