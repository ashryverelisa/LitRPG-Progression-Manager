using SkiaSharp;

namespace ProgressionManager.Controls.Charting;

public static class ChartPaintFactory
{
    public static SKPaint CreateGridPaint(ChartTheme theme)
    {
        return new SKPaint
        {
            Color = theme.GridColor,
            StrokeWidth = theme.GridLineWidth,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };
    }

    public static SKPaint CreateAxisPaint(ChartTheme theme)
    {
        return new SKPaint
        {
            Color = theme.AxisColor,
            StrokeWidth = theme.AxisLineWidth,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };
    }

    public static SKPaint CreateTextPaint(ChartTheme theme, float textSize, SKTextAlign align = SKTextAlign.Left)
    {
        return new SKPaint
        {
            Color = theme.TextColor,
            TextSize = textSize,
            IsAntialias = true,
            TextAlign = align
        };
    }

    public static SKPaint CreateLinePaint(SKColor color, float strokeWidth)
    {
        return new SKPaint
        {
            Color = color,
            StrokeWidth = strokeWidth,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };
    }

    public static SKPaint CreateFillPaint(SKColor color, byte alpha)
    {
        return new SKPaint
        {
            Color = color.WithAlpha(alpha),
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
    }

    public static SKPaint CreatePointPaint(SKColor color)
    {
        return new SKPaint
        {
            Color = color,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
    }
}
