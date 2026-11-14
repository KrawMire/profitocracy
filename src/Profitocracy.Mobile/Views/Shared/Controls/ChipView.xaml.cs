namespace Profitocracy.Mobile.Views.Shared.Controls;

public partial class ChipView : ContentView
{
    public event EventHandler Tapped = (_, _) => { };

    public ChipView()
    {
        InitializeComponent();

        Root.BindingContext = this;
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(ChipView), string.Empty);

    public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
        nameof(IsSelected), typeof(bool), typeof(ChipView), false);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    private void Root_OnTapped(object? sender, TappedEventArgs e)
    {
        Tapped.Invoke(this, EventArgs.Empty);
    }
}
