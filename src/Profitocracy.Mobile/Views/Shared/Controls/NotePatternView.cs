namespace Profitocracy.Mobile.Views.Shared.Controls;

/// <summary>
/// Banknote-like guilloché pattern drawn behind the content of "the note" cards.
/// </summary>
public class NotePatternView : GraphicsView
{
    public NotePatternView()
    {
        InputTransparent = true;
        BackgroundColor = Colors.Transparent;
        Drawable = new NotePatternDrawable(this);
    }

    public static readonly BindableProperty RosetteRingsProperty = BindableProperty.Create(
        nameof(RosetteRings),
        typeof(int),
        typeof(NotePatternView),
        7,
        propertyChanged: (bindable, _, _) => ((NotePatternView)bindable).Invalidate());

    public static readonly BindableProperty PatternColorProperty = BindableProperty.Create(
        nameof(PatternColor),
        typeof(Color),
        typeof(NotePatternView),
        Color.FromArgb("#C8F169"),
        propertyChanged: (bindable, _, _) => ((NotePatternView)bindable).Invalidate());

    /// <summary>
    /// Number of rings of the rosette in the top right corner, zero hides it.
    /// </summary>
    public int RosetteRings
    {
        get => (int)GetValue(RosetteRingsProperty);
        set => SetValue(RosetteRingsProperty, value);
    }

    public Color PatternColor
    {
        get => (Color)GetValue(PatternColorProperty);
        set => SetValue(PatternColorProperty, value);
    }

    private sealed class NotePatternDrawable(NotePatternView view) : IDrawable
    {
        private const float WaveAmplitude = 9;
        private const float RowSpacing = 14;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeSize = 1;
            canvas.StrokeColor = view.PatternColor.WithAlpha(0.09f);

            for (var i = 0; ; i++)
            {
                var y0 = -12 + i * RowSpacing;

                if (y0 - WaveAmplitude > dirtyRect.Height)
                {
                    break;
                }

                canvas.DrawPath(BuildWave(dirtyRect.Width, y0, i * 0.55f));
                canvas.DrawPath(BuildWave(dirtyRect.Width, y0, -i * 0.55f + MathF.PI));
            }

            if (view.RosetteRings <= 0)
            {
                return;
            }

            canvas.StrokeColor = view.PatternColor.WithAlpha(0.22f);

            var centerX = dirtyRect.Width - 40;
            const float centerY = 40;

            for (var k = 0; k < view.RosetteRings; k++)
            {
                var radius = 14 + k * 6;
                var path = new PathF();

                for (var t = 0f; t <= 6.30f; t += 0.05f)
                {
                    var r = radius + 3 * MathF.Sin(12 * t + k * 0.8f);
                    var point = new PointF(centerX + r * MathF.Cos(t), centerY + r * MathF.Sin(t));

                    if (t == 0)
                    {
                        path.MoveTo(point);
                    }
                    else
                    {
                        path.LineTo(point);
                    }
                }

                canvas.DrawPath(path);
            }
        }

        private static PathF BuildWave(float width, float y0, float phase)
        {
            var path = new PathF();

            for (var x = 0f; x <= width + 8; x += 8)
            {
                var point = new PointF(x, y0 + WaveAmplitude * MathF.Sin(x / 30 + phase));

                if (x == 0)
                {
                    path.MoveTo(point);
                }
                else
                {
                    path.LineTo(point);
                }
            }

            return path;
        }
    }
}
