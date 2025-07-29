namespace Profitocracy.Core.Integrations;

/// <summary>
/// Defines methods for importing and exporting backup data within the app.
/// </summary>
public interface IBackupProvider
{
    /// <summary>
    /// Gets the file extension used for backup files
    /// generated or processed by the backup provider.
    /// </summary>
    string BackupFileExtension { get; }

    /// <summary>
    /// Asynchronously imports backup data from the provided stream.
    /// </summary>
    /// <param name="backupFileStream">
    /// The stream containing the backup data to import.
    /// </param>
    /// <returns>
    /// Returns the current index of an importing object
    /// and the total amount of objects to import on each iteration.
    /// </returns>
    IAsyncEnumerable<(int Current, int Total)> ImportDataAsync(Stream backupFileStream);

    /// <summary>
    /// Asynchronously exports backup data to a stream based on the specified inclusion options.
    /// </summary>
    /// <param name="includeProfiles">
    /// Indicates whether to include profile data in the exported backup.
    /// </param>
    /// <param name="includeCategories">
    /// Indicates whether to include category data in the exported backup.
    /// </param>
    /// <param name="includeTransactions">
    /// Indicates whether to include transaction data in the exported backup.
    /// </param>
    /// <returns>
    /// The stream with the exported backup data.
    /// </returns>
    Task<Stream> ExportDataAsync(bool includeProfiles, bool includeCategories, bool includeTransactions);
}