using Profitocracy.Core.Integrations;
using Profitocracy.Infrastructure.Integrations.Models;
using Profitocracy.Infrastructure.Persistence.Sqlite.Configuration;
using Profitocracy.Infrastructure.Persistence.Sqlite.Repositories;
using System.Xml.Serialization;

namespace Profitocracy.Infrastructure.Integrations;

internal sealed class BackupProvider : IBackupProvider
{
    private readonly DbConnection _database;
    private readonly ProfileRepository _profileRepository;
    private readonly CategoryRepository _categoryRepository;
    private readonly TransactionRepository _transactionRepository;

    public string BackupFileExtension => "v1pfbkp";

    public BackupProvider(
        DbConnection database,
        ProfileRepository profileRepository,
        CategoryRepository categoryRepository,
        TransactionRepository transactionRepository)
    {
        _database = database;
        _profileRepository = profileRepository;
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
    }

    public IAsyncEnumerable<(int Current, int Total)> ImportDataAsync(Stream backupFileStream)
    {
        var serializer = new XmlSerializer(typeof(BackupModelV1));

        var abstractModel = serializer.Deserialize(backupFileStream);

        if (abstractModel is null)
        {
            throw new Exception("Deserializing the backup file failed.");
        }

        var model = (BackupModelV1)abstractModel;

        return LoadDataFromBackupAsync(model);
    }

    private async IAsyncEnumerable<(int Current, int Total)> LoadDataFromBackupAsync(BackupModelV1 model)
    {
        if (model.Profiles is null || model.Profiles.Count == 0)
        {
            yield break;
        }

        var totalObjects = model.Profiles.Count;
        totalObjects += model.Categories?.Count ?? 0;
        totalObjects += model.Transactions?.Count ?? 0;

        var currentIndex = 0;

        var newProfileIds = new Dictionary<Guid, Guid>();

        foreach (var profile in model.Profiles)
        {
            var newId = Guid.NewGuid();
            newProfileIds.Add(profile.Id, newId);
            profile.Id = newId;
            profile.IsCurrent = false;

            await _profileRepository.CreateInternal(profile);

            currentIndex++;

            yield return (currentIndex, totalObjects);
        }

        var newCategoryIds = new Dictionary<Guid, Guid>();

        if (model.Categories is not null)
        {
            foreach (var category in model.Categories)
            {
                var newId = Guid.NewGuid();
                newCategoryIds.Add(category.Id, newId);

                var profileId = newProfileIds[category.ProfileId];
                category.Id = newId;
                category.ProfileId = profileId;

                await _categoryRepository.CreateInternal(category);

                currentIndex++;

                yield return (currentIndex, totalObjects);
            }
        }

        if (model.Transactions is not null)
        {
            foreach (var transaction in model.Transactions)
            {
                var newId = Guid.NewGuid();
                var profileId = newProfileIds[transaction.ProfileId];

                if (transaction.CategoryId is not null)
                {
                    var categoryId = newCategoryIds[transaction.CategoryId.Value];
                    transaction.CategoryId = categoryId;
                }

                transaction.Id = newId;
                transaction.ProfileId = profileId;

                await _transactionRepository.CreateInternal(transaction);

                await Task.Delay(5);
                currentIndex++;
                yield return (currentIndex, totalObjects);
            }
        }
    }

    public async Task<Stream> ExportDataAsync(bool includeProfiles, bool includeCategories, bool includeTransactions)
    {
        if (!includeProfiles)
        {
            return Stream.Null;
        }

        var backupModel = new BackupModelV1
        {
            Profiles = await _profileRepository.GetAllProfilesInternal(),
        };

        if (includeCategories)
        {
            backupModel.Categories = [];

            foreach (var profile in backupModel.Profiles)
            {
                var categories = await _categoryRepository.GetAllByProfileIdInternal(profile.Id);
                backupModel.Categories.AddRange(categories);
            }
        }

        if (includeTransactions)
        {
            backupModel.Transactions = [];

            foreach (var profile in backupModel.Profiles)
            {
                var transactions = await _transactionRepository.GetAllByProfileIdInternal(profile.Id);
                backupModel.Transactions.AddRange(transactions);
            }
        }

        var ms = new MemoryStream();
        var serializer = new XmlSerializer(typeof(BackupModelV1));

        serializer.Serialize(ms, backupModel);

        await ms.FlushAsync();
        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }
}