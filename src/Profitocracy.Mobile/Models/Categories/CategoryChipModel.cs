using Profitocracy.Mobile.Abstractions;

namespace Profitocracy.Mobile.Models.Categories;

/// <summary>
/// Category as a selectable chip of the new transaction sheet.
/// </summary>
public class CategoryChipModel : BaseNotifyObject
{
    private bool _isSelected;

    public required CategoryModel Category { get; init; }
    public required string Name { get; init; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
