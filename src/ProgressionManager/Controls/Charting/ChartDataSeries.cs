using System.Collections.Generic;
using SkiaSharp;

namespace ProgressionManager.Controls.Charting;

public class ChartDataSeries
{
    public required string Label { get; init; }
    public required SKColor Color { get; init; }
    public required IReadOnlyList<ChartDataPoint> Points { get; init; }
    public bool ShowFill { get; init; } = true;
    public bool ShowPoints { get; init; } = true;
}