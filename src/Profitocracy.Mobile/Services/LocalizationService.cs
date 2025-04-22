using System.Globalization;
using Profitocracy.Mobile.Resources.Strings;

namespace Profitocracy.Mobile.Services;

public static class LocalizationService
{
    /// <summary>
    /// Constant string value of English language
    /// </summary>
    public const string English = "en";
    
    /// <summary>
    /// Constant string value of Russian language
    /// </summary>
    public const string Russian = "ru";

    /// <summary>
    /// Constant string value of French language
    /// </summary>
    public const string French = "fr";

    /// <summary>
    /// Constant string value of Spanish language
    /// </summary>
    public const string Spanish = "es";
    
    /// <summary>
    /// Constant string value of Serbian language (Cyrillic)
    /// </summary>
    public const string CyrillicSerbian = "sr";
    
    /// <summary>
    /// Constant string value of Serbian language (Latin)
    /// </summary>
    public const string LatinSerbian = "sr-Latn";
    
    /// <summary>
    /// Constant string value for a default language
    /// </summary>
    public const string DefaultLanguage = English;

    /// <summary>
    /// Array of supported languages. Represented by code
    /// </summary>
    public static readonly string[] SupportedLanguages = 
    [
        English, Russian, French, Spanish, CyrillicSerbian, LatinSerbian
    ];
    
    public static string CurrentLanguage => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    
    public static void ChangeCurrentLanguage(string language)
    {
        if (!SupportedLanguages.Contains(language))
        {
            language = DefaultLanguage;
        }
            
        var culture = new CultureInfo(language);
        
        AppResources.Culture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}