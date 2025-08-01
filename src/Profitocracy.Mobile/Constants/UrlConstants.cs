namespace Profitocracy.Mobile.Constants;

public static class UrlConstants
{
    public const string ProjectGitHubUrl = "https://github.com/KrawMire/profitocracy";
    public const string ApplicationId = "com.krawmire.profitocracy";
#if ANDROID
    public const string StoreUrl = "https://play.google.com/store/apps/details?id=com.krawmire.profitocracy";
#elif IOS
    public const string StoreUrl = "https://apps.apple.com/app/profitocracy/id6503658740";
#else
    public const string StoreUrl = "https://play.google.com/store/apps/details?id=com.krawmire.profitocracy";
#endif
}
