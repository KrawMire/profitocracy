using Microsoft.Maui.Controls.Shapes;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace Profitocracy.Mobile.Views.Shared.Controls;

/// <summary>
/// Draws one of the <see cref="Icons"/> line icons: a 24x24 path scaled
/// to <see cref="Size"/> and stroked with <see cref="Color"/>.
/// </summary>
public class IconView : ContentView
{
    private const double GridSize = 24;

    private static readonly PathGeometryConverter GeometryConverter = new();

    private readonly Path _path;

    public IconView()
    {
        InputTransparent = true;

        _path = new Path
        {
            Aspect = Stretch.None,
            StrokeLineCap = PenLineCap.Round,
            StrokeLineJoin = PenLineJoin.Round,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
        };

        Content = _path;
        UpdateSize();
        UpdateColor();
    }

    public static readonly BindableProperty DataProperty = BindableProperty.Create(
        nameof(Data),
        typeof(string),
        typeof(IconView),
        string.Empty,
        propertyChanged: (bindable, _, _) => ((IconView)bindable).UpdateData());

    public static readonly BindableProperty SizeProperty = BindableProperty.Create(
        nameof(Size),
        typeof(double),
        typeof(IconView),
        20d,
        propertyChanged: (bindable, _, _) => ((IconView)bindable).UpdateSize());

    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color),
        typeof(Color),
        typeof(IconView),
        Colors.Black,
        propertyChanged: (bindable, _, _) => ((IconView)bindable).UpdateColor());

    public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create(
        nameof(StrokeWidth),
        typeof(double),
        typeof(IconView),
        1.75d,
        propertyChanged: (bindable, _, _) => ((IconView)bindable).UpdateSize());

    /// <summary>
    /// SVG path data on a 24x24 grid, usually one of <see cref="Icons"/>.
    /// </summary>
    public string Data
    {
        get => (string)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    public double Size
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// Stroke width on the 24x24 grid, it is scaled together with the icon.
    /// </summary>
    public double StrokeWidth
    {
        get => (double)GetValue(StrokeWidthProperty);
        set => SetValue(StrokeWidthProperty, value);
    }

    private void UpdateData()
    {
        _path.Data = string.IsNullOrEmpty(Data)
            ? null
            : (Geometry?)GeometryConverter.ConvertFromInvariantString(Data);
    }

    private void UpdateSize()
    {
        var scale = Size / GridSize;

        WidthRequest = Size;
        HeightRequest = Size;

        _path.WidthRequest = Size;
        _path.HeightRequest = Size;
        _path.RenderTransform = new ScaleTransform(scale, scale);
        _path.StrokeThickness = StrokeWidth * scale;
    }

    private void UpdateColor()
    {
        _path.Stroke = new SolidColorBrush(Color);
    }
}
