using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
using Profitocracy.Core.Persistence;
using Profitocracy.Core.Specifications;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Models.Transactions;
using Profitocracy.Mobile.Resources.Strings;
using System.Collections.ObjectModel;

namespace Profitocracy.Mobile.ViewModels.Transactions;

public class TransactionsPageViewModel : BaseNotifyObject
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly ITransactionService _transactionService;

    private Guid? _profileId;
    private bool _isTransactionsListEmpty;

    public TransactionsPageViewModel(
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

    public async Task Initialize(TransactionsFiltersPageViewModel filters)
    {
        _profileId = await _profileRepository.GetCurrentProfileId();

        if (_profileId is null)
        {
            throw new Exception(AppResources.CommonError_GetCurrentProfile);
        }

        await InitializeTransactions(filters);
    }

    public async Task DeleteTransaction(Guid transactionId)
    {
        var deletedId = await _transactionRepository.Delete(transactionId);

        Transactions.Remove(Transactions.Single(t => t.Id == deletedId));
    }

    public Task<bool> IsTransactionInProfilePeriod(Guid transactionId)
    {
        return _transactionService.CheckTransactionInCurrentPeriod(transactionId);
    }

    private async Task InitializeTransactions(TransactionsFiltersPageViewModel filters)
    {
        if (_profileId is null)
        {
            return;
        }

        var specs = BuildSpecification(_profileId.Value, filters);

        var transactions = await _transactionRepository.GetFiltered(specs);

        Transactions.Clear();
        IsTransactionsListEmpty = transactions.Count == 0;

        foreach (var transaction in transactions)
        {
            Transactions.Add(TransactionModel.FromDomain(transaction));
        }
    }

    private static TransactionsSpecification BuildSpecification(
        Guid profileId,
        TransactionsFiltersPageViewModel filters)
    {
        TransactionType? transactionType = filters.SelectedTransactionTypeIndex switch
        {
            1 => TransactionType.Income,
            2 => TransactionType.Expense,
            _ => null
        };

        SpendingType? spendingType = filters.SelectedSpendingTypeIndex switch
        {
            1 => SpendingType.Main,
            2 => SpendingType.Secondary,
            3 => SpendingType.Saved,
            _ => null
        };

        return new TransactionsSpecification
        {
            ProfileId = profileId,
            FromDate = filters.FromDate,
            ToDate = filters.ToDate,
            CategoryId = filters.SelectedCategory?.Id,
            TransactionType = transactionType,
            SpendingType = spendingType,
            IsMultiCurrency = filters.IsSearchByCurrency,
            Description = filters.Description,
            CurrencyCode = filters.IsSearchByCurrency
                ? filters.SelectedCurrency.Code
                : null,
            IsGreaterThanAmount = filters.IsSearchByAmount && filters.IsGreaterThan,
            Amount = filters.IsSearchByAmount ? filters.Amount : null,
        };
    }
}
