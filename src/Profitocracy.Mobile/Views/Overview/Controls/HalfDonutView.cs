namespace Profitocracy.Mobile.Views.Overview.Controls;

/// <summary>
/// Half-donut gauge of the 50 / 30 / 20 spending types: one arc split into
/// the shares of main, secondary and saved, drawn from left to right over the top.
/// </summary>
public class HalfDonutView : GraphicsView
{
    public const float ChartWidth = 240;
    public const float ChartHeight = 128;

    private const float Thickness = 22;
    private const float Radius = 102;
    private const float CenterY = 118;
    private const float GapDegrees = 1.4f;

    public HalfDonutView()
    {
        InputTransparent = true;
        BackgroundColor = Colors.Transparent;
        WidthRequest = ChartWidth;
        HeightRequest = ChartHeight;
        Drawable = new DonutDrawable(this);
    }

    public static readonly BindableProperty MainShareProperty = CreateProperty(nameof(MainShare), typeof(double), 0d);
    public static readonly BindableProperty SecondaryShareProperty = CreateProperty(nameof(SecondaryShare), typeof(double), 0d);
    public static readonly BindableProperty SavedShareProperty = CreateProperty(nameof(SavedShare), typeof(double), 0d);
    public static readonly BindableProperty MainColorProperty = CreateProperty(nameof(MainColor), typeof(Color), Colors.Green);
    public static readonly BindableProperty SecondaryColorProperty = CreateProperty(nameof(SecondaryColor), typeof(Color), Colors.Orange);
    public static readonly BindableProperty SavedColorProperty = CreateProperty(nameof(SavedColor), typeof(Color), Colors.Blue);
    public static readonly BindableProperty TrackColorProperty = CreateProperty(nameof(TrackColor), typeof(Color), Colors.LightGray);

    /// <summary>
    /// Part of the arc taken by the main spending, from 0 to 1.
    /// </summary>
    public double MainShare
    {
        get => (double)GetValue(MainShareProperty);
        set => SetValue(MainShareProperty, value);
    }

    public double SecondaryShare
    {
        get => (double)GetValue(SecondaryShareProperty);
        set => SetValue(SecondaryShareProperty, value);
    }

    public double SavedShare
    {
        get => (double)GetValue(SavedShareProperty);
        set => SetValue(SavedShareProperty, value);
    }

    public Color MainColor
    {
        get => (Color)GetValue(MainColorProperty);
        set => SetValue(MainColorProperty, value);
    }

    public Color SecondaryColor
    {
        get => (Color)GetValue(SecondaryColorProperty);
        set => SetValue(SecondaryColorProperty, value);
    }

    public Color SavedColor
    {
        get => (Color)GetValue(SavedColorProperty);
        set => SetValue(SavedColorProperty, value);
    }

    /// <summary>
    /// Color of the empty arc, visible when nothing is spent yet.
    /// </summary>
    public Color TrackColor
    {
        get => (Color)GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }

    private static BindableProperty CreateProperty(string name, Type type, object defaultValue)
        => BindableProperty.Create(
            name,
            type,
            typeof(HalfDonutView),
            defaultValue,
            propertyChanged: (bindable, _, _) => ((HalfDonutView)bindable).Invalidate());

    private sealed class DonutDrawable(HalfDonutView view) : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var left = dirtyRect.Center.X - Radius;
            var top = CenterY - Radius;
            var size = Radius * 2;

            canvas.StrokeSize = Thickness;
            canvas.StrokeLineCap = LineCap.Butt;

            // Angles go counter-clockwise from 3 o'clock, so the arc runs from 180° (left) to 0° (right)
            canvas.StrokeColor = view.TrackColor;
            canvas.DrawArc(left, top, size, size, 180, 0, true, false);

            var segments = new[]
                {
                    (Share: view.MainShare, Color: view.MainColor),
                    (Share: view.SecondaryShare, Color: view.SecondaryColor),
                    (Share: view.SavedShare, Color: view.SavedColor),
                }
                .Where(s => s.Share > 0)
                .ToList();

            var total = segments.Sum(s => s.Share);

            if (total <= 0)
            {
                return;
            }

            var gaps = GapDegrees * (segments.Count - 1);
            var available = 180 - gaps;
            var start = 180f;

            foreach (var segment in segments)
            {
                var sweep = (float)(segment.Share / total) * available;

                canvas.StrokeColor = segment.Color;
                canvas.DrawArc(left, top, size, size, start, start - sweep, true, false);

                start -= sweep + GapDegrees;
            }
        }
    }
}
