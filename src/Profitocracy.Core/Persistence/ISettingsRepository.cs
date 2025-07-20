using Profitocracy.Core.Domain.Model.Settings;

namespace Profitocracy.Core.Persistence;

/// <summary>
/// Provides operations for working with
/// persistence layer for settings
/// </summary>
public interface ISettingsRepository
{
    /// <summary>
    /// Gets current settings
    /// </summary>
    /// <returns>Current application settings</returns>
    Task<Settings?> GetCurrentSettings();

    /// <summary>
    /// Create application settings if it does not exist, otherwise, updates
    /// </summary>
    /// <param name="settings">Settings to create or update</param>
    /// <returns>Created or updated instance of settings</returns>
    Task<Settings> CreateOrUpdate(Settings settings);
}