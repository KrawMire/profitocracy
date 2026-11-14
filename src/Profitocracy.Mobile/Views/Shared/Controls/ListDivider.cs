namespace Profitocracy.Mobile.Views.Shared.Controls;

/// <summary>
/// Hairline between rows of a card list. Placed at the top of an item template,
/// it hides itself for the first item, so the card has dividers only between rows.
/// </summary>
public class ListDivider : BoxView
{
    private Element? _item;

    public ListDivider()
    {
        HeightRequest = 1;
        InputTransparent = true;
        this.SetAppThemeColor(
            ColorProperty,
            (Color)Application.Current!.Resources["Line"],
            (Color)Application.Current.Resources["DarkLine"]);
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();

        if (_item is not null)
        {
            _item.ParentChanged -= Item_OnParentChanged;
        }

        _item = Parent;

        if (_item is not null)
        {
            _item.ParentChanged += Item_OnParentChanged;
        }

        UpdateVisibility();
    }

    private void Item_OnParentChanged(object? sender, EventArgs e) => UpdateVisibility();

    private void UpdateVisibility()
    {
        if (_item is IView itemView && _item.Parent is Layout list)
        {
            IsVisible = list.IndexOf(itemView) > 0;
        }
    }
}
