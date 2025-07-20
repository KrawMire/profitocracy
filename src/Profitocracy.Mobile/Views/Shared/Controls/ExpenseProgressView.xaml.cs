namespace Profitocracy.Mobile.Views.Shared.Controls;

public partial class ExpenseProgressView : ContentView
{
    public event EventHandler Clicked = (_, _) => { };

    public ExpenseProgressView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(ModalHeaderView),
        string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty RatioProperty = BindableProperty.Create(
        nameof(Ratio),
        typeof(string),
        typeof(ModalHeaderView),
        string.Empty);

    public string Ratio
    {
        get => (string)GetValue(RatioProperty);
        set => SetValue(RatioProperty, value);
    }

    public static readonly BindableProperty ActualAmountProperty = BindableProperty.Create(
        nameof(ActualAmount),
        typeof(string),
        typeof(ModalHeaderView),
        string.Empty);

    public string ActualAmount
    {
        get => (string)GetValue(ActualAmountProperty);
        set => SetValue(ActualAmountProperty, value);
    }

    public static readonly BindableProperty PlannedAmountProperty = BindableProperty.Create(
        nameof(PlannedAmount),
        typeof(string),
        typeof(ModalHeaderView),
        string.Empty);

    public string PlannedAmount
    {
        get => (string)GetValue(PlannedAmountProperty);
        set => SetValue(PlannedAmountProperty, value);
    }

    private void ExpenseProgressView_OnTapped(object? sender, EventArgs e)
    {
        Clicked.Invoke(this, EventArgs.Empty);
    }
}