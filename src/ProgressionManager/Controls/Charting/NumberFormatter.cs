namespace ProgressionManager.Controls.Charting;

public static class NumberFormatter
{
    public static string FormatCompact(long value)
    {
        return value switch
        {
            >= 1_000_000_000 => $"{value / 1_000_000_000.0:F1}B",
            >= 1_000_000 => $"{value / 1_000_000.0:F1}M",
            >= 1_000 => $"{value / 1_000.0:F1}K",
            _ => value.ToString()
        };
    }

    public static string FormatCompact(double value)
    {
        return value switch
        {
            >= 1_000_000_000 => $"{value / 1_000_000_000.0:F1}B",
            >= 1_000_000 => $"{value / 1_000_000.0:F1}M",
            >= 1_000 => $"{value / 1_000.0:F1}K",
            _ => value.ToString("F0")
        };
    }
}
