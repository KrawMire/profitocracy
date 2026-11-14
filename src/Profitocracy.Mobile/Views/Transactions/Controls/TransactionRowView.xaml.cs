using Profitocracy.Mobile.Models.Transactions;

namespace Profitocracy.Mobile.Views.Transactions.Controls;

public partial class TransactionRowView : ContentView
{
    public TransactionRowView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TransactionProperty = BindableProperty.Create(
        nameof(Transaction),
        typeof(TransactionModel),
        typeof(TransactionRowView),
        propertyChanged: (bindable, _, newValue) => ((TransactionRowView)bindable).Root.BindingContext = newValue);

    public static readonly BindableProperty ShowDayProperty = BindableProperty.Create(
        nameof(ShowDay),
        typeof(bool),
        typeof(TransactionRowView),
        false,
        propertyChanged: (bindable, _, _) => ((TransactionRowView)bindable).UpdateMeta());

    public TransactionModel? Transaction
    {
        get => (TransactionModel?)GetValue(TransactionProperty);
        set => SetValue(TransactionProperty, value);
    }

    /// <summary>
    /// Show the day of the transaction instead of the 50/30/20 word,
    /// e.g. "Food · Today" instead of "Food · needs".
    /// </summary>
    public bool ShowDay
    {
        get => (bool)GetValue(ShowDayProperty);
        set => SetValue(ShowDayProperty, value);
    }

    private void UpdateMeta()
    {
        MetaLabel.SetBinding(
            Label.TextProperty,
            ShowDay
                ? nameof(TransactionModel.MetaWithDay)
                : nameof(TransactionModel.Meta));
    }
}
