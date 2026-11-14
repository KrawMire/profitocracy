using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;
using UIKit;

namespace Profitocracy.Mobile.Platforms.iOS.Renderers;

/// <summary>
/// Shell renderer that lets the tab bar follow the system material (liquid glass on iOS 26+).
/// </summary>
public class AppShellRenderer : ShellRenderer
{
    protected override IShellTabBarAppearanceTracker CreateTabBarAppearanceTracker()
    {
        return new GlassTabBarAppearanceTracker(base.CreateTabBarAppearanceTracker());
    }
}

/// <summary>
/// The tab bar floats over the content, so instead of the solid Surface background of Android
/// it keeps the default system background, with the items tinted with the colors of the current theme.
/// </summary>
internal sealed class GlassTabBarAppearanceTracker(IShellTabBarAppearanceTracker inner) : IShellTabBarAppearanceTracker
{
    public void ResetAppearance(UITabBarController controller)
    {
        inner.ResetAppearance(controller);
    }

    public void SetAppearance(UITabBarController controller, ShellAppearance appearance)
    {
        inner.SetAppearance(controller, appearance);

        var selected = ThemeColor("NoteGreen", "Chartreuse");
        var unselected = ThemeColor("InkSoft", "DarkInkSoft");

        var tabBarAppearance = new UITabBarAppearance();
        tabBarAppearance.ConfigureWithDefaultBackground();

        foreach (var itemAppearance in new[]
                 {
                     tabBarAppearance.StackedLayoutAppearance,
                     tabBarAppearance.InlineLayoutAppearance,
                     tabBarAppearance.CompactInlineLayoutAppearance
                 })
        {
            itemAppearance.Normal.IconColor = unselected;
            itemAppearance.Normal.TitleTextAttributes = new UIStringAttributes { ForegroundColor = unselected };
            itemAppearance.Selected.IconColor = selected;
            itemAppearance.Selected.TitleTextAttributes = new UIStringAttributes { ForegroundColor = selected };
        }

        // Shell paints the bar with its own background color outside of the appearance as well,
        // which hides the system material, so those leftovers are cleared.
        var tabBar = controller.TabBar;
        tabBar.OverrideUserInterfaceStyle = UIUserInterfaceStyle.Unspecified;
        tabBar.BackgroundColor = null;
        tabBar.BarTintColor = null;
        tabBar.StandardAppearance = tabBarAppearance;
        tabBar.ScrollEdgeAppearance = tabBarAppearance;
        tabBar.TintColor = selected;
        tabBar.UnselectedItemTintColor = unselected;

        KeepAddItemColors(controller);
    }

    public void UpdateLayout(UITabBarController controller)
    {
        inner.UpdateLayout(controller);
        KeepAddItemColors(controller);
    }

    /// <summary>
    /// Color that follows the interface style of the view it is drawn in.
    /// </summary>
    private static UIColor ThemeColor(string lightKey, string darkKey)
    {
        var light = ResourceColor(lightKey);
        var dark = ResourceColor(darkKey);

        return UIColor.FromDynamicProvider(traits =>
            traits.UserInterfaceStyle == UIUserInterfaceStyle.Dark ? dark : light);
    }

    private static UIColor ResourceColor(string key)
    {
        return Application.Current?.Resources.TryGetValue(key, out var value) == true && value is Color color
            ? color.ToPlatform()
            : UIColor.Label;
    }

    /// <summary>
    /// The "+" tab in the middle of the bar is a green rounded square, so its image is drawn as is
    /// instead of being tinted. Shell scales tab icons down to the size of the regular icons,
    /// so the image is taken from the bundle at its own (bigger) size.
    /// </summary>
    private static void KeepAddItemColors(UITabBarController controller)
    {
        var items = controller.TabBar.Items;

        if (items is not { Length: 5 })
        {
            return;
        }

        var addItem = items[2];
        AddItemImage ??= UIImage.FromBundle("add_transaction")?.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);

        if (AddItemImage is null || ReferenceEquals(addItem.Image, AddItemImage))
        {
            return;
        }

        addItem.Image = AddItemImage;
        addItem.SelectedImage = AddItemImage;
    }

    private static UIImage? AddItemImage { get; set; }

    public void Dispose()
    {
        inner.Dispose();
    }
}
