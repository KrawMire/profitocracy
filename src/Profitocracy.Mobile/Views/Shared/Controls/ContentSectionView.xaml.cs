namespace Profitocracy.Mobile.Views.Shared.Controls;

public partial class ContentSectionView
{
    public ContentSectionView()
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

    public bool ShowTitle => !string.IsNullOrWhiteSpace(Title);
}
