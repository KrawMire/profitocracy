using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Models.Transactions;
using Profitocracy.Mobile.Resources.Strings;
using System.Collections.ObjectModel;

namespace Profitocracy.Mobile.ViewModels.Transactions;

public class RecurringTransactionsPageViewModel : BaseNotifyObject
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly ITransactionService _transactionService;

    private bool _isTransactionsListEmpty;

    public RecurringTransactionsPageViewModel(
        IProfileRepository profileRepository,
        ITransactionRepository transactionRepository,
        ITransactionService transactionService)
    {
        _transactionRepository = transactionRepository;
        _transactionService = transactionService;
        _profileRepository = profileRepository;
    }

    public readonly ObservableCollection<TransactionModel> Transactions = [];

    public bool IsTransactionsListEmpty
    {
        get => _isTransactionsListEmpty;
        set => SetProperty(ref _isTransactionsListEmpty, value);
    }

    public async Task Initialize()
    {
        var profileId = await _profileRepository.GetCurrentProfileId();

        if (profileId is null)
        {
            throw new Exception(AppResources.CommonError_GetCurrentProfile);
        }

        var transactions = await _transactionRepository.GetRecurringTransactions((Guid)profileId);

        Transactions.Clear();
        IsTransactionsListEmpty = transactions.Count == 0;

        foreach (var transaction in transactions)
        {
            Transactions.Add(TransactionModel.FromDomain(transaction));
        }
    }

    public Task<bool> IsTransactionInProfilePeriod(Guid transactionId)
    {
        return _transactionService.CheckTransactionInCurrentPeriod(transactionId);
    }

    public async Task DeleteTransaction(Guid transactionId)
    {
        var deletedId = await _transactionRepository.Delete(transactionId);

        Transactions.Remove(Transactions.Single(t => t.Id == deletedId));
    }
}
