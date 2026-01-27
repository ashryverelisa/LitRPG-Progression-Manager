namespace ProgressionManager.Controls.Charting;


public readonly struct ChartDataPoint(float x, float y)
{
    public float X { get; init; } = x;
    public float Y { get; init; } = y;
}