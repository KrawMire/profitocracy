using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using Profitocracy.Core;
using Profitocracy.Infrastructure;
using Profitocracy.Mobile.ViewModels.Categories;
using Profitocracy.Mobile.ViewModels.Home;
using Profitocracy.Mobile.ViewModels.Overview;
using Profitocracy.Mobile.ViewModels.Profiles;
using Profitocracy.Mobile.ViewModels.Settings;
using Profitocracy.Mobile.ViewModels.Transactions;
using Profitocracy.Mobile.Views.Home;
using Profitocracy.Mobile.Views.Overview;
using Profitocracy.Mobile.Views.Settings.CategoriesSettings;
using Profitocracy.Mobile.Views.Settings.LanguageSettings;
using Profitocracy.Mobile.Views.Settings.ProfilesSettings;
using Profitocracy.Mobile.Views.Settings.ThemeSettings;
using Profitocracy.Mobile.Views.Transactions;
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
			.AddSingleton<TransactionsPage>();

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
			.AddTransient<HomePageViewModel>()
			.AddTransient<EditTransactionPageViewModel>()
			.AddTransient<FilteredTransactionsPageViewModel>()
			.AddTransient<TransactionsPageViewModel>()
			.AddTransient<ExpenseCategoriesSettingsPageViewModel>()
			.AddTransient<EditExpenseCategoryPageViewModel>()
			.AddTransient<OverviewPageViewModel>()
			.AddTransient<LanguageSettingsViewModel>()
			.AddTransient<ProfileSettingsPageViewModel>()
			.AddTransient<EditProfilePageViewModel>()
			.AddTransient<ThemeSettingsPageViewModel>();
		
		return mauiAppBuilder;
	}

	private static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
	{
		_ = mauiAppBuilder.Services
			.AddTransient<HomePage>()
			.AddTransient<TransactionsPage>()
			.AddTransient<FilteredTransactionsPage>()
			.AddTransient<EditTransactionPage>()
			.AddTransient<ExpenseCategoriesSettingsPage>()
			.AddTransient<EditExpenseCategoryPage>()
			.AddTransient<OverviewPage>()
			.AddTransient<ProfilesSettingsPage>()
			.AddTransient<EditProfilePage>()
			.AddTransient<ThemeSettingsPage>()
			.AddTransient<LanguageSettingsPage>();
		
		return mauiAppBuilder;
	}
}