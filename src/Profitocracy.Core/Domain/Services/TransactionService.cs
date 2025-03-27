using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Persistence;

namespace Profitocracy.Core.Domain.Services;

internal class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IProfileRepository _profileRepository;

    public TransactionService(
        ITransactionRepository transactionRepository, 
        IProfileRepository profileRepository)
    {
        _transactionRepository = transactionRepository;
        _profileRepository = profileRepository;
    }

    public async Task<bool> CheckTransactionInCurrentPeriod(Guid transactionId)
    {
        var transaction = await _transactionRepository.GetById(transactionId);

        if (transaction is null)
        {
            return false;
        }
        
        var profile = await _profileRepository.GetCurrentProfile();

        if (profile is null)
        {
            return false;
        }
        
        return profile.BillingPeriod.DateFrom <= transaction.Timestamp && 
               transaction.Timestamp <= profile.BillingPeriod.DateTo;
    }
}