using CommunityToolkit.Maui;
using Microsoft.Maui.Platform;
using Plugin.LocalNotification;
using Plugin.Maui.AppRating;
using Plugin.Maui.Biometric;
using Profitocracy.Core;
using Profitocracy.Infrastructure;
using Profitocracy.Mobile.Utils;
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
using Profitocracy.Mobile.Views.Shared.Controls;
using Profitocracy.Mobile.Views.Transactions.Pages;
#if DEBUG
using Microsoft.Extensions.Logging;
#endif

namespace Profitocracy.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

#pragma warning disable CA1416 // This call site is reachable on ....
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("SchibstedGrotesk-Regular.ttf", "SansRegular");
                fonts.AddFont("SchibstedGrotesk-Medium.ttf", "SansMedium");
                fonts.AddFont("SchibstedGrotesk-SemiBold.ttf", "SansSemiBold");
                fonts.AddFont("SchibstedGrotesk-Bold.ttf", "SansBold");
                fonts.AddFont("IBMPlexMono-Regular.ttf", "PlexMono");
                fonts.AddFont("IBMPlexMono-Medium.ttf", "PlexMonoMedium");
                fonts.AddFont("Ionicons.ttf", "Ionicons");
            })
            .ConfigureMauiHandlers(handlers =>
            {
#if IOS
                handlers.AddHandler<Shell, Platforms.iOS.Renderers.AppShellRenderer>();
#elif ANDROID
                handlers.AddHandler<Shell, Platforms.Android.Renderers.AppShellRenderer>();
#endif
            })
            .ConfigureEntries()
            .RegisterAppServices()
            .RegisterViewModels()
            .RegisterViews()
            .UseLocalNotification();
#pragma warning restore CA1416

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
            .AddSingleton(BiometricAuthenticationService.Default)
            .AddSingleton(AppRating.Default!);

		var app = builder.Build();
		ServiceHelper.Initialize(app.Services);
		return app;
	}

	/// <summary>
	/// Entries are drawn as outlined rounded fields of the design. A <see cref="BorderlessEntry"/>
	/// sits on a surface tile, so it has neither the outline nor the native border/underline.
	/// Mapped on the background, so the field is redrawn when the theme changes.
	/// </summary>
	private static MauiAppBuilder ConfigureEntries(this MauiAppBuilder mauiAppBuilder)
	{
		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(IView.Background), (handler, view) =>
		{
			var isBorderless = view is BorderlessEntry;
			var fill = (view as VisualElement)?.BackgroundColor ?? Colors.Transparent;
			var stroke = GetThemeColor("Line", "DarkLine");
#if IOS
			var textField = handler.PlatformView;
			textField.BorderStyle = UIKit.UITextBorderStyle.None;

			if (isBorderless)
			{
				return;
			}

			textField.Layer.CornerRadius = 12;
			textField.Layer.BorderWidth = 1;
			textField.Layer.BorderColor = stroke.ToCGColor();
			textField.BackgroundColor = fill.ToPlatform();
			textField.LeftView = new UIKit.UIView(new CoreGraphics.CGRect(0, 0, 12, 1));
			textField.LeftViewMode = UIKit.UITextFieldViewMode.Always;
			textField.RightView = new UIKit.UIView(new CoreGraphics.CGRect(0, 0, 12, 1));
			textField.RightViewMode = UIKit.UITextFieldViewMode.Always;
#elif ANDROID
			var editText = handler.PlatformView;

			if (isBorderless)
			{
				editText.BackgroundTintList =
					Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
				return;
			}

			var context = editText.Context!;
			var field = new Android.Graphics.Drawables.GradientDrawable();
			field.SetCornerRadius(context.ToPixels(12));
			field.SetColor(fill.ToPlatform());
			field.SetStroke((int)context.ToPixels(1), stroke.ToPlatform());

			editText.Background = field;
			var horizontal = (int)context.ToPixels(12);
			var vertical = (int)context.ToPixels(10);
			editText.SetPadding(horizontal, vertical, horizontal, vertical);
#endif
		});

		return mauiAppBuilder;
	}

	private static Color GetThemeColor(string lightKey, string darkKey)
	{
		var app = Application.Current;
		var key = app?.RequestedTheme == AppTheme.Dark ? darkKey : lightKey;

		return app?.Resources.TryGetValue(key, out var value) == true && value is Color color
			? color
			: Colors.Gray;
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
            .AddTransient<RecurringTransactionsPageViewModel>()
            .AddTransient<ExpenseCategoriesSettingsPageViewModel>()
            .AddTransient<EditExpenseCategoryPageViewModel>()
            .AddTransient<OverviewPageViewModel>()
            .AddTransient<OverviewFiltersPageViewModel>()
            .AddTransient<LanguageSettingsViewModel>()
            .AddTransient<ProfileSettingsPageViewModel>()
            .AddTransient<TransactionsFiltersPageViewModel>()
            .AddTransient<ImportExportSettingsPageViewModel>()
            .AddTransient<EditProfilePageViewModel>()
            .AddTransient<ThemeSettingsPageViewModel>()
            .AddTransient<SettingsPageViewModel>()
            .AddTransient<NotificationsSettingsPageViewModel>()
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
            .AddTransient<RecurringTransactionsPage>()
            .AddTransient<FilteredTransactionsPage>()
            .AddTransient<EditTransactionPage>()
            .AddTransient<ExpenseCategoriesSettingsPage>()
            .AddTransient<EditExpenseCategoryPage>()
            .AddTransient<OverviewPage>()
            .AddTransient<ProfilesSettingsPage>()
            .AddTransient<EditProfilePage>()
            .AddTransient<NotificationsSettingsPage>()
            .AddTransient<ThemeSettingsPage>()
            .AddTransient<ImportExportSettingsPage>()
            .AddTransient<LanguageSettingsPage>()
            .AddTransient<AuthSettingsPage>()
            .AddTransient<SettingsPage>();

        return mauiAppBuilder;
    }
}
