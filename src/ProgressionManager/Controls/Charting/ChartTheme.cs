using SkiaSharp;

namespace ProgressionManager.Controls.Charting;

public class ChartTheme
{
    public SKColor BackgroundColor { get; init; } = new(10, 12, 16);
    public SKColor GridColor { get; init; } = new(42, 48, 66);
    public SKColor TextColor { get; init; } = new(192, 202, 245);
    public SKColor AxisColor { get; init; } = new(65, 72, 104);

    public float GridLineWidth { get; init; } = 1f;
    public float AxisLineWidth { get; init; } = 2f;
    public float DataLineWidth { get; init; } = 3f;
    public float DataPointRadius { get; init; } = 4f;
    public float LegendPointRadius { get; init; } = 5f;

    public float AxisLabelTextSize { get; init; } = 11f;
    public float LegendTextSize { get; init; } = 12f;
    public float NoDataTextSize { get; init; } = 14f;

    public byte FillAlpha { get; init; } = 40;

    public static ChartTheme Dark { get; } = new();
}
