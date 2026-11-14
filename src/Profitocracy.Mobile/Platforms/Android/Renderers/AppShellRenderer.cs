using Android.Graphics.Drawables;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;

namespace Profitocracy.Mobile.Platforms.Android.Renderers;

/// <summary>
/// Shell renderer that draws the "+" tab in the middle of the bottom bar as a green rounded square.
/// </summary>
public class AppShellRenderer : ShellRenderer
{
    protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
    {
        return new AddItemBottomNavViewAppearanceTracker(base.CreateBottomNavViewAppearanceTracker(shellItem));
    }
}

/// <summary>
/// The bottom bar tints every icon with the item colors, which turns the square of the "+" tab
/// into a flat silhouette. The tint is removed from that item only, and its icon is made bigger.
/// The bar also gets a hairline on top that separates it from the page.
/// </summary>
internal sealed class AddItemBottomNavViewAppearanceTracker(IShellBottomNavViewAppearanceTracker inner)
    : IShellBottomNavViewAppearanceTracker
{
    private const int AddItemIndex = 2;
    private const int AddItemIconSize = 40;

    public void ResetAppearance(BottomNavigationView bottomView)
    {
        inner.ResetAppearance(bottomView);
        KeepAddItemColors(bottomView);
    }

    public void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
    {
        inner.SetAppearance(bottomView, appearance);
        DrawTopLine(bottomView);
        KeepAddItemColors(bottomView);
    }

    /// <summary>
    /// Surface of the bar with a 1dp line of the Line color along its top edge.
    /// </summary>
    private static void DrawTopLine(BottomNavigationView bottomView)
    {
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        var line = new ColorDrawable(ResourceColor(isDark ? "DarkLine" : "Line"));
        var surface = new ColorDrawable(ResourceColor(isDark ? "DarkSurface" : "Surface"));
        var background = new LayerDrawable([line, surface]);

        background.SetLayerInset(1, 0, (int)bottomView.Context.ToPixels(1), 0, 0);
        bottomView.Background = background;
        bottomView.Elevation = 0;
    }

    private static global::Android.Graphics.Color ResourceColor(string key)
    {
        return Application.Current?.Resources.TryGetValue(key, out var value) == true && value is Color color
            ? color.ToPlatform()
            : global::Android.Graphics.Color.Transparent;
    }

    private static void KeepAddItemColors(BottomNavigationView bottomView)
    {
        Apply(bottomView);

        // The menu of the bar is filled after the appearance is set for the first time
        bottomView.Post(() => Apply(bottomView));
    }

    private static void Apply(BottomNavigationView bottomView)
    {
        if (bottomView.GetChildAt(0) is not NavigationBarMenuView menuView ||
            menuView.ChildCount <= AddItemIndex ||
            menuView.GetChildAt(AddItemIndex) is not NavigationBarItemView addItemView)
        {
            return;
        }

        addItemView.SetIconTintList(null);
        addItemView.SetIconSize((int)bottomView.Context.ToPixels(AddItemIconSize));
    }

    public void Dispose()
    {
        inner.Dispose();
    }
}
