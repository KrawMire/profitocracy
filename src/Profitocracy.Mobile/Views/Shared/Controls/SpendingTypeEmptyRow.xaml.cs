namespace Profitocracy.Mobile.Views.Shared.Controls;

public partial class SpendingTypeEmptyRow : ContentView
{
    public SpendingTypeEmptyRow()
    {
        InitializeComponent();

        Root.BindingContext = this;
    }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(SpendingTypeEmptyRow), string.Empty);

    public static readonly BindableProperty WordProperty = BindableProperty.Create(
        nameof(Word), typeof(string), typeof(SpendingTypeEmptyRow), string.Empty);

    public static readonly BindableProperty DotColorProperty = BindableProperty.Create(
        nameof(DotColor), typeof(Color), typeof(SpendingTypeEmptyRow), Colors.Transparent);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Word of the split under the title: "needs", "wants" or "savings".
    /// </summary>
    public string Word
    {
        get => (string)GetValue(WordProperty);
        set => SetValue(WordProperty, value);
    }

    public Color DotColor
    {
        get => (Color)GetValue(DotColorProperty);
        set => SetValue(DotColorProperty, value);
    }
}
