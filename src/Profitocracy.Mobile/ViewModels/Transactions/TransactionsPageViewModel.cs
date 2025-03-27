using System.Collections.ObjectModel;
using Profitocracy.Core.Domain.Abstractions.Services;
using Profitocracy.Core.Persistence;
using Profitocracy.Core.Specifications;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Models.Transactions;
using Profitocracy.Mobile.Resources.Strings;

namespace Profitocracy.Mobile.ViewModels.Transactions;

public class TransactionsPageViewModel : BaseNotifyObject
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly ITransactionService _transactionService;
    
    private Guid? _profileId;
    private DateTime _fromDate;
    private DateTime _toDate;
    
    public TransactionsPageViewModel(
        IProfileRepository profileRepository,
        ITransactionRepository transactionRepository, 
        ITransactionService transactionService)
    {
        _transactionRepository = transactionRepository;
        _transactionService = transactionService;
        _profileRepository = profileRepository;
        
        var currentDate = DateTime.Now;
        
        _fromDate = new DateTime(currentDate.Year, currentDate.Month, 1);
        _toDate = new DateTime(
            currentDate.Year,
            currentDate.Month,
            day: DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
    }

    public DateTime FromDate
    {
        get => _fromDate;
        set
        {
            SetProperty(ref _fromDate, value);
            _ = InitializeTransactions();
        }
    }
    
    public DateTime ToDate
    {
        get => _toDate;
        set
        {
            var newValue = new DateTime(
                value.Year, 
                value.Month, 
                value.Day,
                hour: 0,
                minute: 0,
                second: 0,
                millisecond: 0);
            
            SetProperty(ref _toDate, newValue);
            _ = InitializeTransactions();
        }
    }
    
    public readonly ObservableCollection<TransactionModel> Transactions = [];

    public async Task Initialize()
    {
        _profileId = await _profileRepository.GetCurrentProfileId();

        if (_profileId is null)
        {
            throw new Exception(AppResources.CommonError_GetCurrentProfile);
        }
        
        await InitializeTransactions();
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

    private async Task InitializeTransactions()
    {
        if (_profileId is null)
        {
            return;
        }

        var specs = new TransactionsSpecification
        {
            ProfileId = _profileId,
            FromDate = _fromDate,
            ToDate = _toDate
        };
        
        var transactions = await _transactionRepository.GetFiltered(specs);
        
        Transactions.Clear();

        foreach (var transaction in transactions)
        {
            Transactions.Add(TransactionModel.FromDomain(transaction));
        }
    }
}