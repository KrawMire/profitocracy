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
    /// <summary>
    /// Decimal separator of the stored amounts (invariant culture)
    /// </summary>
    private const string DecimalPoint = ".";

    private const int MaxDigits = 8;
    private const int MaxDecimals = 2;

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
    private Currency _profileCurrency = Currency.AvailableCurrencies.DefaultCurrency;
    private string _profileName = string.Empty;

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
    private bool _isEditingProfileAmount;
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
    public ObservableCollection<CategoryChipModel> CategoryChips { get; } = [];
    public List<RecurringTransactionIntervalModel> AvailableIntervals { get; } = [.. Enum.GetValues<RecurringTransactionInterval>().Cast<RecurringTransactionInterval>().Select(RecurringTransactionIntervalModel.FromDomain)];
    public CategoryModel? Category { get; set; }

    public Currency ProfileCurrency => _profileCurrency;

    public RecurringTransactionIntervalModel? SelectedInterval
    {
        get => _selectedInterval;
        set
        {
            SetProperty(ref _selectedInterval, value);
            OnPropertyChanged(nameof(IntervalText));
        }
    }

    public Currency SelectedCurrency
    {
        get => _selectedCurrency;
        set
        {
            SetProperty(ref _selectedCurrency, value);
            OnAmountsChanged();
        }
    }

    public Guid TransactionId
    {
        set => _transactionId = value;
    }

    public bool IsEditing => _transactionId is not null;

    public string Title => _transactionId is null
        ? AppResources.NewTransaction_Title
        : AppResources.NewTransaction_EditTitle;

    public bool IsIncome
    {
        get => _isIncome;
        set
        {
            SetProperty(ref _isIncome, value);
            OnPropertyChanged(nameof(IncomeHint));
        }
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
            SetProperty(ref _isMultiCurrency, value);

            if (!value)
            {
                _isEditingProfileAmount = false;
            }

            OnPropertyChanged(nameof(IncomeHint));
            OnAmountsChanged();
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

    /// <summary>
    /// Amount in the currency of the profile, as typed (invariant culture).
    /// </summary>
    public string Amount
    {
        get => _amount;
        set
        {
            SetProperty(ref _amount, value);
            OnAmountsChanged();
        }
    }

    /// <summary>
    /// Amount in the other currency of a multi-currency transaction (invariant culture).
    /// </summary>
    public string DestinationAmount
    {
        get => _destinationAmount;
        set
        {
            SetProperty(ref _destinationAmount, value);
            OnAmountsChanged();
        }
    }

    public DateTime Timestamp
    {
        get => _timestamp;
        set
        {
            SetProperty(ref _timestamp, value);
            OnPropertyChanged(nameof(DateText));
        }
    }

    public TimeSpan Time
    {
        get => _time;
        set
        {
            SetProperty(ref _time, value);
            OnPropertyChanged(nameof(TimeText));
        }
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

    /// <summary>
    /// The amount in the field, in the currency of <see cref="EditedCurrencyCode"/>:
    /// the other currency of a multi-currency transaction, or the currency of the profile.
    /// Typed with the system number pad, both "." and "," are taken as the decimal separator.
    /// </summary>
    public string EditedAmount
    {
        get
        {
            var value = IsEditingProfileAmount ? _amount : _destinationAmount;
            return value.Replace(DecimalPoint, CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        }
        set
        {
            var normalized = NormalizeTyped(value);

            if (normalized is null)
            {
                // Not an amount: the field goes back to the previous value
                OnPropertyChanged();
                return;
            }

            if (IsEditingProfileAmount)
            {
                Amount = normalized;
            }
            else
            {
                DestinationAmount = normalized;
            }
        }
    }

    /// <summary>
    /// Code of the currency the field is typed in, e.g. "USD".
    /// </summary>
    public string EditedCurrencyCode => IsEditingProfileAmount
        ? _profileCurrency.Code
        : SelectedCurrency.Code;

    /// <summary>
    /// The other side of a multi-currency transaction with the exchange rate,
    /// e.g. "= €287.01 in EUR Profile · 1 USD = 0.9567 EUR". Tapping it moves the field there.
    /// </summary>
    public string ConvertedText
    {
        get
        {
            if (!IsMultiCurrency)
            {
                return string.Empty;
            }

            string converted;

            if (_isEditingProfileAmount)
            {
                converted = string.Format(
                    AppResources.NewTransaction_InProfile,
                    FormatTyped(_destinationAmount, SelectedCurrency.Symbol),
                    SelectedCurrency.Code);
            }
            else if (NumberUtils.TryParseDecimal(_amount, out var amount) && amount > 0)
            {
                converted = string.Format(
                    AppResources.NewTransaction_InProfile,
                    FormatTyped(_amount, _profileCurrency.Symbol),
                    _profileName);
            }
            else
            {
                return string.Format(AppResources.NewTransaction_EnterProfileAmount, _profileCurrency.Code);
            }

            var rate = RateText;

            return string.IsNullOrEmpty(rate) ? converted : $"{converted} · {rate}";
        }
    }

    /// <summary>
    /// Exchange rate that follows from the two typed amounts, e.g. "1 USD = 0.9567 EUR".
    /// </summary>
    private string RateText
    {
        get
        {
            if (!IsMultiCurrency
                || !NumberUtils.TryParseDecimal(_amount, out var amount)
                || !NumberUtils.TryParseDecimal(_destinationAmount, out var destination)
                || amount <= 0
                || destination <= 0)
            {
                return string.Empty;
            }

            var rate = Math.Round(amount / destination, 4);

            return string.Format(
                AppResources.NewTransaction_Rate,
                SelectedCurrency.Code,
                $"{rate.ToString("0.####", CultureInfo.CurrentCulture)} {_profileCurrency.Code}");
        }
    }

    private bool IsEditingProfileAmount => !IsMultiCurrency || _isEditingProfileAmount;

    public string SaveText => string.Format(
        AppResources.NewTransaction_Save,
        NumberUtils.TryParseDecimal(_amount, out var amount)
            ? NumberUtils.FormatMoney(amount, _profileCurrency.Symbol)
            : NumberUtils.FormatMoney(0, _profileCurrency.Symbol));

    public string IncomeHint => IsMultiCurrency
        ? string.Format(AppResources.NewTransaction_WithdrawHint, SelectedCurrency.Code)
        : AppResources.NewTransaction_IncomeHint;

    public string DateText => _timestamp.Date == DateTime.Today
        ? $"{AppResources.Common_Today}, {_timestamp.ToString("d MMM", CultureInfo.CurrentCulture)}"
        : _timestamp.ToString("ddd, d MMM", CultureInfo.CurrentCulture);

    public string TimeText => DateTime.Today.Add(_time).ToString("t", CultureInfo.CurrentCulture);

    public string IntervalText => SelectedInterval is null || SelectedInterval.Value <= 0
        ? AppResources.NewTransaction_NoRepeat
        : SelectedInterval.Name;

    public async Task Initialize()
    {
        var profile = await _profileRepository.GetCurrentProfile();

        if (profile is null)
        {
            throw new Exception(AppResources.CommonError_GetCurrentProfile);
        }

        _profileCurrency = profile.Settings.Currency;
        _profileName = profile.Name;

        SelectedCurrency = AvailableCurrencies.FirstOrDefault(c => c != _profileCurrency) ?? AvailableCurrencies[0];

        var categories = await _categoryRepository.GetAllByProfileId(profile.Id);

        AvailableCategories.Clear();
        AvailableCategories.Add(NoneCategory);

        foreach (var category in categories)
        {
            AvailableCategories.Add(CategoryModel.FromDomain(category));
        }

        if (_transactionId is not null)
        {
            await InitializeEditableTransaction();
        }

        CategoryChips.Clear();

        foreach (var category in AvailableCategories)
        {
            CategoryChips.Add(new CategoryChipModel
            {
                Category = category,
                Name = category == NoneCategory ? AppResources.NewTransaction_NoCategory : category.Name,
                IsSelected = category == (Category ?? NoneCategory),
            });
        }

        OnPropertyChanged(nameof(Title));
        OnAmountsChanged();
    }

    public void SelectCategory(CategoryChipModel chip)
    {
        Category = chip.Category == NoneCategory ? null : chip.Category;

        foreach (var categoryChip in CategoryChips)
        {
            categoryChip.IsSelected = categoryChip == chip;
        }
    }

    /// <summary>
    /// Chooses the currency the money was paid in. The currency of the profile
    /// makes a regular transaction, any other one makes a multi-currency transaction.
    /// </summary>
    public void SelectCurrency(Currency currency)
    {
        if (currency == _profileCurrency)
        {
            IsMultiCurrency = false;
        }
        else
        {
            SelectedCurrency = currency;
            IsMultiCurrency = true;
        }

        OnAmountsChanged();
    }

    /// <summary>
    /// Moves the amount field between the two amounts of a multi-currency transaction.
    /// </summary>
    public void SwitchEditedAmount()
    {
        if (!IsMultiCurrency)
        {
            return;
        }

        _isEditingProfileAmount = !_isEditingProfileAmount;
        OnAmountsChanged();
    }

    /// <summary>
    /// Brings a typed amount to the invariant culture and checks it: digits with
    /// one decimal separator, at most <see cref="MaxDigits"/> digits and <see cref="MaxDecimals"/> decimals.
    /// </summary>
    /// <returns>The amount with "." as the decimal separator, or null if it is not an amount</returns>
    private static string? NormalizeTyped(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value
            .Trim()
            .Replace(",", DecimalPoint);

        var parts = normalized.Split(DecimalPoint);

        if (parts.Length > 2
            || !parts.All(p => p.All(char.IsAsciiDigit))
            || (parts.Length == 2 && parts[1].Length > MaxDecimals)
            || normalized.Replace(DecimalPoint, string.Empty).Length > MaxDigits)
        {
            return null;
        }

        // "05" is typed as "5", a lone separator as "0."
        var integer = parts[0].TrimStart('0');
        integer = integer.Length == 0 ? "0" : integer;

        if (parts.Length == 1)
        {
            return parts[0].Length == 0 ? string.Empty : integer;
        }

        return $"{integer}{DecimalPoint}{parts[1]}";
    }

    /// <summary>
    /// Formats an amount that is being typed: group separators for
    /// the integer part, the decimals exactly as typed, e.g. "€1,234.5".
    /// </summary>
    private static string FormatTyped(string value, string symbol)
    {
        var parts = value.Split(DecimalPoint);
        var integer = long.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var number)
            ? number.ToString("N0", CultureInfo.CurrentCulture)
            : "0";

        return parts.Length > 1
            ? $"{symbol}{integer}{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}{parts[1]}"
            : $"{symbol}{integer}";
    }

    private void OnAmountsChanged()
    {
        OnPropertyChanged(nameof(EditedAmount));
        OnPropertyChanged(nameof(EditedCurrencyCode));
        OnPropertyChanged(nameof(ConvertedText));
        OnPropertyChanged(nameof(SaveText));
        OnPropertyChanged(nameof(IncomeHint));
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
        Amount = FormatForField(transaction.Amount);
        Timestamp = transaction.Timestamp;
        Time = transaction.Timestamp.TimeOfDay;
        Description = transaction.Description!;

        if (transaction.Category is not null)
        {
            Category = AvailableCategories.FirstOrDefault(c => c.Id == transaction.Category.Id);
        }

        if (transaction is MultiCurrencyTransaction multiCurrencyTransaction)
        {
            SelectedCurrency = multiCurrencyTransaction.DestinationCurrency;
            DestinationAmount = FormatForField(multiCurrencyTransaction.DestinationAmount);
            IsMultiCurrency = true;
        }

        if (transaction.RecurringTransactionInfo is not null &&
            transaction.RecurringTransactionInfo.Interval is not RecurringTransactionInterval.None)
        {
            SelectedInterval = AvailableIntervals.FirstOrDefault(i => i.Value == (short)transaction.RecurringTransactionInfo.Interval);
        }
    }

    private static string FormatForField(decimal amount)
        => Math.Round(amount, MaxDecimals).ToString("0.##", CultureInfo.InvariantCulture);

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
        var isWithdrawal = IsMultiCurrency && _transactionType == 0;

        if (!NumberUtils.TryParseDecimal(_amount, out var amount)
            || amount < 0
            || (amount == 0 && !isWithdrawal))
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
                Name = Category.Name,
            };
        }

        RecurringTransactionInfo? recurringTransactionInfo = null;

        if (SelectedInterval is not null)
        {
            recurringTransactionInfo = new RecurringTransactionInfo
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
            currentProfile.Settings.Currency,
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
        if (!NumberUtils.TryParseDecimal(_destinationAmount, out var destinationAmount) || destinationAmount <= 0)
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
            _ => TransactionDestination.Expense,
        };

        var transactionTimestamp = _timestamp.Date.Add(_time);

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
            transactionTimestamp,
            _description,
            geoTag: null,
            category,
            recurringTransactionInfo);
    }
}
