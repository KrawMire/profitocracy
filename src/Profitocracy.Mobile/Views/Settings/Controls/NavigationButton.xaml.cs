namespace Profitocracy.Mobile.Views.Settings.Controls;

public partial class NavigationButton : ContentView
{
    public event EventHandler Clicked = (_, _) => {};
    
    public NavigationButton()
    {
        InitializeComponent();
        
        BindingContext = this;
    }
    
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), 
        typeof(string), 
        typeof(NavigationButton), 
        string.Empty);

    public string Title
    {
        get => (string) GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    private void InternalNavigationButton_OnClicked(object? sender, EventArgs e)
    {
        Clicked.Invoke(this, EventArgs.Empty);
    }
}