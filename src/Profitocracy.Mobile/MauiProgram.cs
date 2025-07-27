using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Biometric;
using Profitocracy.Core;
using Profitocracy.Infrastructure;
using Profitocracy.Mobile.ViewModels.Auth;
using Profitocracy.Mobile.ViewModels.Categories;
using Profitocracy.Mobile.ViewModels.Home;
using Profitocracy.Mobile.ViewModels.Overview;
using Profitocracy.Mobile.ViewModels.Profiles;
using Profitocracy.Mobile.ViewModels.Settings;
using Profitocracy.Mobile.ViewModels.Transactions;
using Profitocracy.Mobile.Views.Auth;
using Profitocracy.Mobile.Views.Home.Pages;
using Profitocracy.Mobile.Views.Overview.Pages;
using Profitocracy.Mobile.Views.Settings.Pages;
using Profitocracy.Mobile.Views.Transactions.Pages;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Profitocracy.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseSkiaSharp()
            .UseLiveCharts()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Ionicons.ttf", "Ionicons");
            })
            .RegisterAppServices()
            .RegisterViewModels()
            .RegisterViews();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        var infrastructureConfig = new InfrastructureConfiguration
        {
            AppDirectoryPath = FileSystem.AppDataDirectory
        };

        builder.Services
            .RegisterInfrastructureServices(infrastructureConfig)
            .RegisterCoreServices()
            .AddSingleton(_ => BiometricAuthenticationService.Default);

        return builder.Build();
    }

    private static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
    {
        _ = mauiAppBuilder.Services
            .AddSingleton<AppShell>()
            .AddSingleton<AppInit>();

        return mauiAppBuilder;
    }

    private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
        _ = mauiAppBuilder.Services
            .AddTransient<AuthPageViewModel>()
            .AddTransient<HomePageViewModel>()
            .AddTransient<NewPeriodSelectionPageViewModel>()
            .AddTransient<EditTransactionPageViewModel>()
            .AddTransient<FilteredTransactionsPageViewModel>()
            .AddTransient<TransactionsPageViewModel>()
            .AddTransient<ExpenseCategoriesSettingsPageViewModel>()
            .AddTransient<EditExpenseCategoryPageViewModel>()
            .AddTransient<OverviewPageViewModel>()
            .AddTransient<OverviewFiltersPageViewModel>()
            .AddTransient<LanguageSettingsViewModel>()
            .AddTransient<ProfileSettingsPageViewModel>()
            .AddTransient<TransactionsFiltersPageViewModel>()
            .AddTransient<EditProfilePageViewModel>()
            .AddTransient<ThemeSettingsPageViewModel>()
            .AddTransient<AuthSettingsPageViewModel>();

        return mauiAppBuilder;
    }

    private static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
    {
        _ = mauiAppBuilder.Services
            .AddTransient<AuthPage>()
            .AddTransient<HomePage>()
            .AddTransient<NewPeriodSelectionPage>()
            .AddTransient<TransactionsPage>()
            .AddTransient<FilteredTransactionsPage>()
            .AddTransient<EditTransactionPage>()
            .AddTransient<ExpenseCategoriesSettingsPage>()
            .AddTransient<EditExpenseCategoryPage>()
            .AddTransient<OverviewPage>()
            .AddTransient<ProfilesSettingsPage>()
            .AddTransient<EditProfilePage>()
            .AddTransient<ThemeSettingsPage>()
            .AddTransient<LanguageSettingsPage>()
            .AddTransient<AuthSettingsPage>();

        return mauiAppBuilder;
    }
}