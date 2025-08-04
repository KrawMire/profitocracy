using Profitocracy.Infrastructure.Persistence.Sqlite.Models.Category;
using Profitocracy.Infrastructure.Persistence.Sqlite.Models.Profile;
using Profitocracy.Infrastructure.Persistence.Sqlite.Models.Settings;
using Profitocracy.Infrastructure.Persistence.Sqlite.Models.Transaction;
using SQLite;

namespace Profitocracy.Infrastructure.Persistence.Sqlite.Configuration;

internal class DbConnection
{
    private SQLiteAsyncConnection? _database;
    private readonly InfrastructureConfiguration _configuration;

    private const int DatabaseVersionV0 = 0;
    private const int DatabaseVersionV1 = 1;
    private const int CurrentDatabaseVersion = DatabaseVersionV1;

    public DbConnection(InfrastructureConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public SQLiteAsyncConnection Database
    {
        get
        {
            if (_database is null)
            {
                throw new NullReferenceException("Database connection is null. Need to call Init() before using DbConnection");
            }

            return _database;
        }
    }

    public async ValueTask Init()
    {
        if (_database is not null)
        {
            return;
        }

        _database = new SQLiteAsyncConnection(GetDatabasePath(Constants.DatabaseFilename), Constants.Flags);
        await InitializeDatabase();
    }

    private string GetDatabasePath(string filename)
    {
        return Path.Combine(_configuration.AppDirectoryPath, filename);
    }

    private async Task InitializeDatabase()
    {
        if (_database is null)
        {
            throw new NullReferenceException("Local DB connection is not initialized");
        }

        await CreateTables();

        var version = await GetDatabaseVersion();

        if (version != CurrentDatabaseVersion)
        {
            await PerformMigration(version);
        }
    }

    private async Task CreateTables()
    {
        if (_database is null)
        {
            throw new NullReferenceException("Local DB connection is not initialized");
        }

        await _database.CreateTableAsync<DatabaseVersion>();
        await _database.CreateTableAsync<TransactionModel>();
        await _database.CreateTableAsync<CategoryModel>();
        await _database.CreateTableAsync<ProfileModel>();
        await _database.CreateTableAsync<SettingsModel>();
    }

    private async Task<int> GetDatabaseVersion()
    {
        var version = await _database!.Table<DatabaseVersion>().FirstOrDefaultAsync();
        return version?.Version ?? 0;
    }

    private async Task PerformMigration(int currentVersion)
    {
        while (currentVersion < CurrentDatabaseVersion)
        {
            switch (currentVersion)
            {
                case DatabaseVersionV0:
                    await PerformMigrationToV1();
                    break;
            }

            currentVersion++;
        }
    }

    private async Task PerformMigrationToV1()
    {
        if (_database is null)
        {
            throw new NullReferenceException("Local DB connection is not initialized");
        }

        var profiles = await _database
            .Table<ProfileModel>()
            .ToListAsync();

        foreach (var profile in profiles)
        {
            var profileTransactions = await _database
                .Table<TransactionModel>()
                .Where(t => t.ProfileId == profile.Id)
                .ToListAsync();

            foreach (var transaction in profileTransactions)
            {
                transaction.SourceCurrencyCode = profile.CurrencyCode;
                await _database.UpdateAsync(transaction);
            }
        }

        await SetDatabaseVersion(DatabaseVersionV1);
    }

    private async Task SetDatabaseVersion(int version)
    {
        await _database!
            .Table<DatabaseVersion>()
            .DeleteAsync(v => true);

        var dbVersion = new DatabaseVersion { Version = version };
        await _database!.InsertAsync(dbVersion);
    }
}
