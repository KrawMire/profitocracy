using Profitocracy.Core.Domain.Model.Profiles;
using Profitocracy.Core.Domain.Model.Shared.ValueObjects;
using Profitocracy.Core.Domain.Model.Transactions;
using Profitocracy.Core.Domain.Model.Transactions.Entities;
using Profitocracy.Core.Domain.Model.Transactions.Factories;
using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
using Profitocracy.Core.Persistence;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Models.Categories;
using Profitocracy.Mobile.Models.Transactions;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.Utils;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Profitocracy.Mobile.ViewModels.Transactions;

public class EditTransactionPageViewModel : BaseNotifyObject
{
    private readonly IProfileRepository _profileRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;

    private static readonly CategoryModel NoneCategory = new()
    {
        Id = Guid.NewGuid(),
        ProfileId = Guid.Empty,
        Name = AppResources.AddTransaction_NoneCategory
    };

    private Guid? _transactionId;

    private DateTime _timestamp = DateTime.Now;
    private TimeSpan _time = DateTime.Now.TimeOfDay;
    private string _amount = string.Empty;
    private string _destinationAmount = string.Empty;

    private Currency _selectedCurrency;
    private string? _description;
    private int _transactionType;
    private int? _spendingType;

    private bool _isIncome;
    private bool _isExpense;
    private bool _isMain;
    private bool _isSecondary;
    private bool _isSaved;

    private bool _isMultiCurrency;
    private RecurringTransactionIntervalModel? _selectedInterval;
    
    public EditTransactionPageViewModel(
        IProfileRepository profileRepository,
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository)
    {
        _profileRepository = profileRepository;
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;

        _transactionId = null;

        _transactionType = 1;
        _spendingType = 0;

        _isIncome = false;

        _isExpense = true;
        _isMain = true;
        _isSecondary = false;
        _isSaved = false;

        _isMultiCurrency = false;

        foreach (var currency in Currency.AvailableCurrencies.All.Values)
        {
            AvailableCurrencies.Add(currency);
        }

        _selectedCurrency = AvailableCurrencies[0];
    }

    public ObservableCollection<Currency> AvailableCurrencies { get; } = [];
    public ObservableCollection<CategoryModel> AvailableCategories { get; } = [];
    public List<RecurringTransactionIntervalModel> AvailableIntervals { get; } = [.. Enum.GetValues<RecurringTransactionInterval>().Cast<RecurringTransactionInterval>().Select(RecurringTransactionIntervalModel.FromDomain)];
    public CategoryModel? Category { get; set; }
    public RecurringTransactionIntervalModel? SelectedInterval
    {
        get => _selectedInterval;
        set => SetProperty(ref _selectedInterval, value);
    }

    public Currency SelectedCurrency
    {
        get => _selectedCurrency;
        set => SetProperty(ref _selectedCurrency, value);
    }

    public Guid TransactionId
    {
        set => _transactionId = value;
    }

    public bool IsIncome
    {
        get => _isIncome;
        set => SetProperty(ref _isIncome, value);
    }

    public bool IsExpense
    {
        get => _isExpense;
        set => SetProperty(ref _isExpense, value);
    }

    public bool IsMain
    {
        get => _isMain;
        set => SetProperty(ref _isMain, value);
    }

    public bool IsSecondary
    {
        get => _isSecondary;
        set => SetProperty(ref _isSecondary, value);
    }

    public bool IsSaved
    {
        get => _isSaved;
        set => SetProperty(ref _isSaved, value);
    }

    public bool IsMultiCurrency
    {
        get => _isMultiCurrency;
        set
        {
            if (_isMultiCurrency == value)
            {
                return;
            }

            _isMultiCurrency = value;

            if (!_isMultiCurrency)
            {
                SpendingType = 0;
            }

            OnPropertyChanged();
        }
    }


    public int TransactionType
    {
        get => _transactionType;
        set
        {
            if (_transactionType == value)
            {
                return;
            }

            _transactionType = value;

            switch (value)
            {
                case 0:
                    IsIncome = true;
                    IsExpense = false;
                    SpendingType = null;
                    break;
                case 1:
                    IsIncome = false;
                    IsExpense = true;
                    SpendingType = 0;
                    break;
            }

            OnPropertyChanged();
        }
    }

    public int? SpendingType
    {
        get => _spendingType;
        set
        {
            if (_spendingType == value)
            {
                return;
            }

            _spendingType = value;

            switch (value)
            {
                case 1:
                    IsMain = false;
                    IsSecondary = true;
                    IsSaved = false;
                    break;
                case 2:
                    IsMain = false;
                    IsSecondary = false;
                    IsSaved = true;
                    IsMultiCurrency = true;
                    break;
                default:
                    IsMain = true;
                    IsSecondary = false;
                    IsSaved = false;
                    break;
            }

            OnPropertyChanged();
        }
    }

    public string Amount
    {
        get => _amount;
        set => SetProperty(ref _amount, value);
    }

    public string DestinationAmount
    {
        get => _destinationAmount;
        set => SetProperty(ref _destinationAmount, value);
    }

    public DateTime Timestamp
    {
        get => _timestamp;
        set => SetProperty(ref _timestamp, value);
    }

    public TimeSpan Time
    {
        get => _time;
        set => SetProperty(ref _time, value);
    }

    public string Description
    {
        get => _description ?? string.Empty;
        set
        {
            _description = string.IsNullOrWhiteSpace(value) ? null : value;
            OnPropertyChanged();
        }
    }

    public async Task Initialize()
    {
        var profileId = await _profileRepository.GetCurrentProfileId();

        if (profileId is null)
        {
            throw new Exception(AppResources.CommonError_GetCurrentProfile);
        }

        SelectedCurrency = AvailableCurrencies[0];

        var categories = await _categoryRepository.GetAllByProfileId((Guid)profileId);

        AvailableCategories.Add(NoneCategory);

        foreach (var category in categories)
        {
            AvailableCategories.Add(CategoryModel.FromDomain(category));
        }

        if (_transactionId is not null)
        {
            await InitializeEditableTransaction();
        }
    }

    private async Task InitializeEditableTransaction()
    {
        // Using null-forgiving operator because caller have already done null check
        var transaction = await _transactionRepository.GetById((Guid)_transactionId!);

        if (transaction is null)
        {
            throw new ArgumentNullException(AppResources.CommonError_FindTransactionToEdit);
        }

        var isIncomeTransaction =
            transaction.Type == Core.Domain.Model.Transactions.ValueObjects.TransactionType.Income;

        IsIncome = isIncomeTransaction;
        IsExpense = !isIncomeTransaction;
        IsMain = transaction.SpendingType == Core.Domain.Model.Shared.ValueObjects.SpendingType.Main;
        IsSecondary = transaction.SpendingType == Core.Domain.Model.Shared.ValueObjects.SpendingType.Secondary;
        IsSaved = transaction.SpendingType == Core.Domain.Model.Shared.ValueObjects.SpendingType.Saved;

        TransactionType = (int)transaction.Type;
        SpendingType = isIncomeTransaction ? null : (int?)transaction.SpendingType;
        Amount = transaction.Amount.ToString(CultureInfo.CurrentCulture);
        Timestamp = transaction.Timestamp;
        Time = transaction.Timestamp.TimeOfDay;
        Description = transaction.Description!;

        if (transaction.Category is not null)
        {
            Category = AvailableCategories.FirstOrDefault(c => c.Id == transaction.Category.Id);
        }

        if (transaction is MultiCurrencyTransaction multiCurrencyTransaction)
        {
            IsMultiCurrency = true;
            SelectedCurrency = multiCurrencyTransaction.DestinationCurrency;
            DestinationAmount = multiCurrencyTransaction.DestinationAmount.ToString(CultureInfo.CurrentCulture);
        }

        if (transaction.RecurringTransactionInfo is not null &&
            transaction.RecurringTransactionInfo.Interval is not RecurringTransactionInterval.None)
        {
            SelectedInterval = AvailableIntervals.FirstOrDefault(i => i.Value == (short)transaction.RecurringTransactionInfo.Interval);
        }
    }

    public Task SaveTransaction()
    {
        return _transactionId is null
            ? CreateTransaction()
            : UpdateTransaction();
    }

    private async Task CreateTransaction()
    {
        var transaction = await BuildTransaction(transactionId: null);
        await _transactionRepository.Create(transaction);
    }

    private async Task UpdateTransaction()
    {
        var transaction = await BuildTransaction(_transactionId);
        await _transactionRepository.Update(transaction);
    }

    private async Task<Transaction> BuildTransaction(Guid? transactionId)
    {
        _amount = _amount.Replace(",", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);

        if (!NumberUtils.TryParseDecimal(_amount, out var amount))
        {
            throw new InvalidCastException(AppResources.CommonError_AmountNumber);
        }

        if (_transactionType < 0)
        {
            throw new IndexOutOfRangeException(AppResources.CommonError_TransactionType);
        }

        var currentProfile = await _profileRepository.GetCurrentProfile();

        if (currentProfile is null)
        {
            throw new ArgumentNullException(AppResources.CommonError_GetCurrentProfile);
        }

        TransactionCategory? category = null;

        if (Category?.Id is not null && !Category.Id.Equals(NoneCategory.Id))
        {
            category = new TransactionCategory((Guid)Category.Id)
            {
                Name = Category.Name
            };
        }

        RecurringTransactionInfo? recurringTransactionInfo = null;

        if (SelectedInterval is not null)
        {
            recurringTransactionInfo = new RecurringTransactionInfo() 
            {
                Interval = (RecurringTransactionInterval)SelectedInterval.Value
            };
        }

        if (IsMultiCurrency)
        {
            return BuildMultiCurrencyTransaction(transactionId, amount, currentProfile, category, recurringTransactionInfo);
        }

        var transactionTimestamp = _timestamp.Date.Add(_time);

        return TransactionFactory.CreateTransaction(
            id: transactionId,
            amount,
            currentProfile.Id,
            (TransactionType)_transactionType,
            _spendingType is null or -1 ? null : (SpendingType)_spendingType,
            transactionTimestamp,
            _description,
            geoTag: null,
            category,
            recurringTransactionInfo);
    }

    private MultiCurrencyTransaction BuildMultiCurrencyTransaction(
        Guid? transactionId,
        decimal amount,
        Profile profile,
        TransactionCategory? category,
        RecurringTransactionInfo? recurringTransactionInfo)
    {
        if (!NumberUtils.TryParseDecimal(_destinationAmount, out var destinationAmount))
        {
            throw new InvalidCastException(AppResources.CommonError_OriginalCurrencyAmountNumber);
        }

        var destination = _transactionType switch
        {
            // Transaction type is Income and Multi-currency, so
            // it is taking from saved amounts to profile balance
            0 => TransactionDestination.ProfileBalance,
            // Transaction type is Expense, Multi-currency and spending
            // type is saved, so it is saving funds in another currency
            1 when _spendingType == 2 => TransactionDestination.SavingsBalance,
            _ => TransactionDestination.Expense
        };

        return TransactionFactory.CreateMultiCurrencyTransaction(
            id: transactionId,
            amount,
            destinationAmount,
            profile.Settings.Currency,
            SelectedCurrency,
            profile.Id,
            (TransactionType)_transactionType,
            _spendingType is null or -1 ? null : (SpendingType)_spendingType,
            destination,
            _timestamp,
            _description,
            geoTag: null,
            category,
            recurringTransactionInfo);
    }
}
