namespace Profitocracy.Mobile.Views.Shared.Controls;

public partial class ModalHeaderView : ContentView
{
    public event EventHandler CloseClicked = (_, _) => { };

    public ModalHeaderView()
    {
        InitializeComponent();

        BindingContext = this;
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

    private void CloseButton_OnClicked(object? sender, EventArgs e)
    {
        CloseClicked.Invoke(this, EventArgs.Empty);
    }
}