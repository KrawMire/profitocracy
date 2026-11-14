namespace Profitocracy.Mobile.Views.Shared.Controls;

/// <summary>
/// Pill-shaped progress meter of the design system with an optional limit mark.
/// </summary>
public class MeterView : GraphicsView
{
    private const float MarkWidth = 2;
    private const float MarkOverhang = 4;

    public MeterView()
    {
        InputTransparent = true;
        BackgroundColor = Colors.Transparent;
        Drawable = new MeterDrawable(this);
        UpdateHeight();
    }

    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(
        nameof(Progress),
        typeof(double),
        typeof(MeterView),
        0d,
        propertyChanged: (bindable, _, _) => ((MeterView)bindable).Invalidate());

    public static readonly BindableProperty MarkProperty = BindableProperty.Create(
        nameof(Mark),
        typeof(double),
        typeof(MeterView),
        -1d,
        propertyChanged: (bindable, _, _) => ((MeterView)bindable).OnMarkChanged());

    public static readonly BindableProperty BarHeightProperty = BindableProperty.Create(
        nameof(BarHeight),
        typeof(double),
        typeof(MeterView),
        4d,
        propertyChanged: (bindable, _, _) => ((MeterView)bindable).UpdateHeight());

    public static readonly BindableProperty TrackColorProperty = BindableProperty.Create(
        nameof(TrackColor),
        typeof(Color),
        typeof(MeterView),
        Color.FromArgb("#E4E8DD"),
        propertyChanged: (bindable, _, _) => ((MeterView)bindable).Invalidate());

    public static readonly BindableProperty FillColorProperty = BindableProperty.Create(
        nameof(FillColor),
        typeof(Color),
        typeof(MeterView),
        Color.FromArgb("#10241B"),
        propertyChanged: (bindable, _, _) => ((MeterView)bindable).Invalidate());

    public static readonly BindableProperty MarkColorProperty = BindableProperty.Create(
        nameof(MarkColor),
        typeof(Color),
        typeof(MeterView),
        Color.FromArgb("#10241B"),
        propertyChanged: (bindable, _, _) => ((MeterView)bindable).Invalidate());

    /// <summary>
    /// Filled part of the meter, from 0 to 1 (bigger values are clamped).
    /// </summary>
    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    /// <summary>
    /// Position of the limit mark, from 0 to 1. Negative value hides the mark.
    /// </summary>
    public double Mark
    {
        get => (double)GetValue(MarkProperty);
        set => SetValue(MarkProperty, value);
    }

    public double BarHeight
    {
        get => (double)GetValue(BarHeightProperty);
        set => SetValue(BarHeightProperty, value);
    }

    public Color TrackColor
    {
        get => (Color)GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }

    public Color FillColor
    {
        get => (Color)GetValue(FillColorProperty);
        set => SetValue(FillColorProperty, value);
    }

    public Color MarkColor
    {
        get => (Color)GetValue(MarkColorProperty);
        set => SetValue(MarkColorProperty, value);
    }

    private void OnMarkChanged()
    {
        UpdateHeight();
        Invalidate();
    }

    private void UpdateHeight()
    {
        HeightRequest = Mark >= 0
            ? BarHeight + MarkOverhang * 2
            : BarHeight;

        Invalidate();
    }

    private sealed class MeterDrawable(MeterView view) : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var barHeight = (float)view.BarHeight;
            var top = (dirtyRect.Height - barHeight) / 2;
            var radius = barHeight / 2;
            var progress = (float)Math.Clamp(view.Progress, 0, 1);

            canvas.FillColor = view.TrackColor;
            canvas.FillRoundedRectangle(0, top, dirtyRect.Width, barHeight, radius);

            if (progress > 0)
            {
                // Keep a visible dot for tiny values, the same way the design does
                var width = Math.Max(dirtyRect.Width * progress, barHeight);

                canvas.FillColor = view.FillColor;
                canvas.FillRoundedRectangle(0, top, width, barHeight, radius);
            }

            if (view.Mark < 0)
            {
                return;
            }

            var markX = (float)Math.Clamp(view.Mark, 0, 0.995) * dirtyRect.Width;

            canvas.FillColor = view.MarkColor;
            canvas.FillRectangle(markX, 0, MarkWidth, dirtyRect.Height);
        }
    }
}
