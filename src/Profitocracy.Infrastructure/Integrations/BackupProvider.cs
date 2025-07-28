using Profitocracy.Core.Integrations;
using System.Text;

namespace Profitocracy.Infrastructure.Integrations;

internal sealed class BackupProvider : IBackupProvider
{
    public string BackupFileExtension => "pfc.v1.backup";

    public Task ImportDataAsync(Stream backupFileStream)
    {
        return Task.CompletedTask;
    }

    public Task<Stream> ExportDataAsync(bool includeProfiles, bool includeCategories, bool includeTransactions)
    {
        const string testStr = "Hello, this is a test backup file.";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(testStr));
        return Task.FromResult<Stream>(stream);
    }
}