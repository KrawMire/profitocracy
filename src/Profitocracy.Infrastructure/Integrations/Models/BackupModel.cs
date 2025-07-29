using Profitocracy.Infrastructure.Persistence.Sqlite.Models.Category;
using Profitocracy.Infrastructure.Persistence.Sqlite.Models.Profile;
using Profitocracy.Infrastructure.Persistence.Sqlite.Models.Transaction;

namespace Profitocracy.Infrastructure.Integrations.Models;

public class BackupModelV1
{
    public List<ProfileModel>? Profiles { get; set; }
    public List<CategoryModel>? Categories { get; set; }
    public List<TransactionModel>? Transactions { get; set; }
}
