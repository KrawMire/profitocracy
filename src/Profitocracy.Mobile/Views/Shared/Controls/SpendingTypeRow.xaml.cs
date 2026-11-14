namespace Profitocracy.Mobile.Views.Shared.Controls;

public partial class SpendingTypeRow : ContentView
{
    public event EventHandler Tapped = (_, _) => { };

    public SpendingTypeRow()
    {
        InitializeComponent();

        // Only the inner layout binds to the control,
        // so bindings set on the control itself keep the outer context
        Root.BindingContext = this;
    }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(SpendingTypeRow), string.Empty);

    public static readonly BindableProperty AmountProperty = BindableProperty.Create(
        nameof(Amount), typeof(string), typeof(SpendingTypeRow), string.Empty);

    public static readonly BindableProperty DetailProperty = BindableProperty.Create(
        nameof(Detail), typeof(string), typeof(SpendingTypeRow), string.Empty);

    public static readonly BindableProperty RemainingProperty = BindableProperty.Create(
        nameof(Remaining), typeof(string), typeof(SpendingTypeRow), string.Empty);

    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(
        nameof(Progress), typeof(double), typeof(SpendingTypeRow), 0d);

    public static readonly BindableProperty DotColorProperty = BindableProperty.Create(
        nameof(DotColor), typeof(Color), typeof(SpendingTypeRow), Colors.Transparent);

    public static readonly BindableProperty TintColorProperty = BindableProperty.Create(
        nameof(TintColor), typeof(Color), typeof(SpendingTypeRow), Colors.Transparent);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Amount
    {
        get => (string)GetValue(AmountProperty);
        set => SetValue(AmountProperty, value);
    }

    public string Detail
    {
        get => (string)GetValue(DetailProperty);
        set => SetValue(DetailProperty, value);
    }

    public string Remaining
    {
        get => (string)GetValue(RemainingProperty);
        set => SetValue(RemainingProperty, value);
    }

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public Color DotColor
    {
        get => (Color)GetValue(DotColorProperty);
        set => SetValue(DotColorProperty, value);
    }

    public Color TintColor
    {
        get => (Color)GetValue(TintColorProperty);
        set => SetValue(TintColorProperty, value);
    }

    private void Root_OnTapped(object? sender, TappedEventArgs e)
    {
        Tapped.Invoke(this, EventArgs.Empty);
    }
}
