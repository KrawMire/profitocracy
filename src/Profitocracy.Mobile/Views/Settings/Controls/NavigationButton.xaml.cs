namespace Profitocracy.Mobile.Views.Settings.Controls;

public partial class NavigationButton : ContentView
{
    public event EventHandler Clicked = (_, _) => { };

    public NavigationButton()
    {
        InitializeComponent();

        // Only the inner layout binds to the control,
        // so bindings set on the control itself keep the outer context
        Root.BindingContext = this;
    }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(NavigationButton),
        string.Empty);

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(string),
        typeof(NavigationButton),
        string.Empty,
        propertyChanged: (bindable, _, _) => ((NavigationButton)bindable).OnPropertyChanged(nameof(HasValue)));

    public static readonly BindableProperty AccessoryProperty = BindableProperty.Create(
        nameof(Accessory),
        typeof(View),
        typeof(NavigationButton));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Current value shown before the chevron, e.g. "Light".
    /// </summary>
    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Custom view shown before the chevron instead of the value.
    /// </summary>
    public View? Accessory
    {
        get => (View?)GetValue(AccessoryProperty);
        set => SetValue(AccessoryProperty, value);
    }

    public bool HasValue => !string.IsNullOrEmpty(Value);

    private void InternalNavigationButton_OnClicked(object? sender, EventArgs e)
    {
        Clicked.Invoke(this, EventArgs.Empty);
    }
}
