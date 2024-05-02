﻿using Microsoft.Extensions.Logging;
using Profitocracy.BusinessLogic;
using Profitocracy.Domain.Boundaries.CategoryBoundary.Aggregate;
using Profitocracy.Domain.Boundaries.ProfileBoundary.Aggregate;
using Profitocracy.Domain.Boundaries.TransactionBoundary.Aggregate;
using Profitocracy.Infrastructure;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Mappers;
using Profitocracy.Mobile.Models.Category;
using Profitocracy.Mobile.Models.Profile;
using Profitocracy.Mobile.Models.Transaction;
using Profitocracy.Mobile.ViewModels.Categories;
using Profitocracy.Mobile.ViewModels.Home;
using Profitocracy.Mobile.ViewModels.Setup;
using Profitocracy.Mobile.ViewModels.Transactions;
using Profitocracy.Mobile.Views.Pages.Home;
using Profitocracy.Mobile.Views.Pages.Transactions;
using Profitocracy.Mobile.Views.Settings.CategoriesSettings;
using Profitocracy.Mobile.Views.Setup;
using Profitocracy.Mobile.Views.Transactions;

namespace Profitocracy.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.RegisterAppServices()
			.RegisterViewModels()
			.RegisterViews()
			.RegisterModels()
			.RegisterPresentationMappers();

		
#if DEBUG
		builder.Logging.AddDebug();
#endif

		var infrastructureConfig = new InfrastructureConfiguration
		{
			AppDirectoryPath = FileSystem.AppDataDirectory 
		};
		
		builder.Services
			.RegisterInfrastructureServices(infrastructureConfig)
			.RegisterServices()
			.AddSingleton<TransactionsPage>();

		return builder.Build();
	}
	
	private static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
	{
		_ = mauiAppBuilder.Services.AddSingleton<AppShell>();

		return mauiAppBuilder;
	}

	private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
	{
		_ = mauiAppBuilder.Services
			.AddTransient<HomePageViewModel>()
			.AddTransient<SetupPageViewModel>()
			.AddTransient<AddTransactionPageViewModel>()
			.AddTransient<TransactionPageViewModel>()
			.AddTransient<ExpenseCategoriesSettingsPageViewModel>()
			.AddTransient<AddExpenseCategoryPageViewModel>();
		
		return mauiAppBuilder;
	}

	private static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
	{
		_ = mauiAppBuilder.Services
			.AddSingleton<HomePage>()
			.AddSingleton<SetupPage>()
			.AddSingleton<TransactionsPage>()
			.AddTransient<AddTransactionPage>()
			.AddTransient<ExpenseCategoriesSettingsPage>()
			.AddTransient<AddExpenseCategoryPage>();
		
		return mauiAppBuilder;
	}

	private static MauiAppBuilder RegisterModels(this MauiAppBuilder mauiAppBuilder)
	{
		return mauiAppBuilder;
	}
	
	private static MauiAppBuilder RegisterPresentationMappers(this MauiAppBuilder mauiAppBuilder)
	{
		_ = mauiAppBuilder.Services
			.AddTransient<IPresentationMapper<Profile, ProfileModel>, ProfileMapper>()
			.AddTransient<IPresentationMapper<Transaction, TransactionModel>, TransactionMapper>()
			.AddTransient<IPresentationMapper<Category, CategoryModel>, CategoryMapper>();
		
		return mauiAppBuilder;
	} 
}