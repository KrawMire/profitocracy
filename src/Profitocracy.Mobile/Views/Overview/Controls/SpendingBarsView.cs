using Microsoft.Maui.Graphics.Text;
using Font = Microsoft.Maui.Graphics.Font;

namespace Profitocracy.Mobile.Views.Overview.Controls;

/// <summary>
/// One bar of the <see cref="SpendingBarsView"/>
/// </summary>
/// <param name="Amount">Spent amount</param>
/// <param name="IsFuture">The day (or week) has not come yet</param>
/// <param name="Label">Caption under the bar, empty for most of the bars</param>
public record SpendingBar(decimal Amount, bool IsFuture, string Label);

/// <summary>
/// Spending by day (or week) with the affordable average as a dashed line.
/// Bars above the average are red, the biggest one gets a note on top.
/// The scale ends at twice the average: taller bars are cut short with a break near their top.
/// </summary>
public class SpendingBarsView : GraphicsView
{
    /// <summary>
    /// Bars above this many averages are cut short.
    /// </summary>
    public const decimal CapAverages = 2;

    private const float ChartHeight = 176;
    private const float AxisHeight = 24;
    private const float NoteHeight = 30;
    private const float BarGap = 3;
    private const float MinBarHeight = 2;
    private const float ClippedOverhang = 6;
    private const float ClippedTop = 12;
    private const float ClippedBreak = 4;

    public SpendingBarsView()
    {
        InputTransparent = true;
        BackgroundColor = Colors.Transparent;
        HeightRequest = ChartHeight + AxisHeight;
        Drawable = new BarsDrawable(this);
    }

    public static readonly BindableProperty BarsProperty = CreateProperty(nameof(Bars), typeof(IReadOnlyList<SpendingBar>), null);
    public static readonly BindableProperty AverageProperty = CreateProperty(nameof(Average), typeof(decimal), 0m);
    public static readonly BindableProperty AverageTextProperty = CreateProperty(nameof(AverageText), typeof(string), string.Empty);
    public static readonly BindableProperty NoteTextProperty = CreateProperty(nameof(NoteText), typeof(string), string.Empty);
    public static readonly BindableProperty BarColorProperty = CreateProperty(nameof(BarColor), typeof(Color), Colors.Gray);
    public static readonly BindableProperty OverColorProperty = CreateProperty(nameof(OverColor), typeof(Color), Colors.Red);
    public static readonly BindableProperty NoteColorProperty = CreateProperty(nameof(NoteColor), typeof(Color), Colors.Red);
    public static readonly BindableProperty EmptyColorProperty = CreateProperty(nameof(EmptyColor), typeof(Color), Colors.LightGray);
    public static readonly BindableProperty TextColorProperty = CreateProperty(nameof(TextColor), typeof(Color), Colors.Gray);
    public static readonly BindableProperty SurfaceColorProperty = CreateProperty(nameof(SurfaceColor), typeof(Color), Colors.White);

    public IReadOnlyList<SpendingBar>? Bars
    {
        get => (IReadOnlyList<SpendingBar>?)GetValue(BarsProperty);
        set => SetValue(BarsProperty, value);
    }

    /// <summary>
    /// Amount that can be spent per bar without overspending.
    /// </summary>
    public decimal Average
    {
        get => (decimal)GetValue(AverageProperty);
        set => SetValue(AverageProperty, value);
    }

    public string AverageText
    {
        get => (string)GetValue(AverageTextProperty);
        set => SetValue(AverageTextProperty, value);
    }

    /// <summary>
    /// Note above the biggest bar, e.g. "€750 · Taxes + Loans".
    /// </summary>
    public string NoteText
    {
        get => (string)GetValue(NoteTextProperty);
        set => SetValue(NoteTextProperty, value);
    }

    public Color BarColor
    {
        get => (Color)GetValue(BarColorProperty);
        set => SetValue(BarColorProperty, value);
    }

    public Color OverColor
    {
        get => (Color)GetValue(OverColorProperty);
        set => SetValue(OverColorProperty, value);
    }

    public Color NoteColor
    {
        get => (Color)GetValue(NoteColorProperty);
        set => SetValue(NoteColorProperty, value);
    }

    public Color EmptyColor
    {
        get => (Color)GetValue(EmptyColorProperty);
        set => SetValue(EmptyColorProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>
    /// Background of the card, used behind the average label.
    /// </summary>
    public Color SurfaceColor
    {
        get => (Color)GetValue(SurfaceColorProperty);
        set => SetValue(SurfaceColorProperty, value);
    }

    private static BindableProperty CreateProperty(string name, Type type, object? defaultValue)
        => BindableProperty.Create(
            name,
            type,
            typeof(SpendingBarsView),
            defaultValue,
            propertyChanged: (bindable, _, _) => ((SpendingBarsView)bindable).Invalidate());

    private sealed class BarsDrawable(SpendingBarsView view) : IDrawable
    {
        private static readonly Font SansFont = new("SchibstedGrotesk-Regular");
        private static readonly Font SansMediumFont = new("SchibstedGrotesk-Medium");
        private static readonly Font SansStrongFont = new("SchibstedGrotesk-SemiBold");

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var bars = view.Bars;

            if (bars is null || bars.Count == 0)
            {
                return;
            }

            var width = dirtyRect.Width;
            var chartBottom = ChartHeight;
            var plotHeight = ChartHeight - NoteHeight;
            var max = view.Average > 0
                ? view.Average * CapAverages
                : bars.Max(b => b.Amount);
            max = max <= 0 ? 1 : max;

            // Baseline
            canvas.StrokeColor = view.EmptyColor;
            canvas.StrokeSize = 1;
            canvas.DrawLine(0, chartBottom, width, chartBottom);

            // Bars
            var barWidth = (width - BarGap * (bars.Count - 1)) / bars.Count;
            var biggest = bars.Select((b, i) => (Bar: b, Index: i)).MaxBy(b => b.Bar.Amount);

            for (var i = 0; i < bars.Count; i++)
            {
                var bar = bars[i];
                var isEmpty = bar.IsFuture || bar.Amount <= 0;
                var isClipped = !isEmpty && bar.Amount > max;
                var height = isEmpty
                    ? MinBarHeight
                    : isClipped
                        ? plotHeight + ClippedOverhang
                        : Math.Max(MinBarHeight, (float)(bar.Amount / max) * plotHeight);
                var x = i * (barWidth + BarGap);
                var top = chartBottom - height;

                canvas.FillColor = isEmpty
                    ? view.EmptyColor
                    : view.Average > 0 && bar.Amount > view.Average
                        ? view.OverColor
                        : view.BarColor;

                if (isClipped)
                {
                    var restTop = top + ClippedTop + ClippedBreak;

                    canvas.FillRoundedRectangle(x, top, barWidth, ClippedTop, 2, 2, 0, 0);
                    canvas.FillRectangle(x, restTop, barWidth, chartBottom - restTop);
                }
                else
                {
                    canvas.FillRoundedRectangle(x, top, barWidth, height, 2, 2, 0, 0);
                }
            }

            // Average line with its label on the right
            if (view.Average > 0)
            {
                var y = chartBottom - (float)(view.Average / max) * plotHeight;

                canvas.StrokeColor = view.TextColor;
                canvas.StrokeSize = 1.5f;
                canvas.StrokeDashPattern = [4, 3];
                canvas.DrawLine(0, y, width, y);
                canvas.StrokeDashPattern = null;

                if (!string.IsNullOrEmpty(view.AverageText))
                {
                    canvas.Font = SansMediumFont;
                    canvas.FontSize = 12;

                    var size = canvas.GetStringSize(view.AverageText, SansMediumFont, 12);
                    var labelRect = new RectF(width - size.Width - 4, y - 18, size.Width + 4, 16);

                    canvas.FillColor = view.SurfaceColor;
                    canvas.FillRectangle(labelRect);
                    canvas.FontColor = view.TextColor;
                    canvas.DrawString(view.AverageText, labelRect, HorizontalAlignment.Right, VerticalAlignment.Center);
                }
            }

            // Note above the biggest bar
            if (!string.IsNullOrEmpty(view.NoteText) && biggest.Bar.Amount > 0)
            {
                canvas.Font = SansStrongFont;
                canvas.FontSize = 12;
                canvas.FontColor = view.NoteColor;

                var size = canvas.GetStringSize(view.NoteText, SansStrongFont, 12);
                var x = biggest.Index * (barWidth + BarGap) + barWidth + 6;
                x = Math.Clamp(x, 0, Math.Max(0, width - size.Width));

                canvas.DrawString(view.NoteText, new RectF(x, 0, width - x, 16), HorizontalAlignment.Left, VerticalAlignment.Center, TextFlow.ClipBounds);
            }

            // Captions of the axis
            canvas.Font = SansFont;
            canvas.FontSize = 12;
            canvas.FontColor = view.TextColor;

            for (var i = 0; i < bars.Count; i++)
            {
                var label = bars[i].Label;

                if (string.IsNullOrEmpty(label))
                {
                    continue;
                }

                var size = canvas.GetStringSize(label, SansFont, 12);
                var center = i * (barWidth + BarGap) + barWidth / 2;
                var x = Math.Clamp(center - size.Width / 2, 0, Math.Max(0, width - size.Width));

                canvas.DrawString(label, new RectF(x, chartBottom + 6, size.Width + 2, 16), HorizontalAlignment.Left, VerticalAlignment.Center);
            }
        }
    }
}
