using Profitocracy.Mobile.Models.Transactions;

namespace Profitocracy.Mobile.Views.Transactions.Controls;

public partial class TransactionView : ContentView
{ 
    public TransactionView()
    {
        InitializeComponent();
    }
    
    public static readonly BindableProperty TransactionProperty = BindableProperty.Create(
        nameof(Transaction), 
        typeof(TransactionModel), 
        typeof(TransactionView));
    
    public TransactionModel Transaction
    {
        get => (TransactionModel)GetValue(TransactionProperty); 
        set => SetValue(TransactionProperty, value);
    }
}