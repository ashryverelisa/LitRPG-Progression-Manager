namespace ProgressionManager.Controls.Charting;

public readonly struct ChartPadding(float left, float top, float right, float bottom)
{
    public float Left { get; init; } = left;
    public float Right { get; init; } = right;
    public float Top { get; init; } = top;
    public float Bottom { get; init; } = bottom;

    public ChartPadding(float horizontal, float vertical)
        : this(horizontal, vertical, horizontal, vertical) { }

    public ChartPadding(float uniform)
        : this(uniform, uniform, uniform, uniform) { }

    public static ChartPadding Default { get; } = new(50f, 20f, 20f, 40f);
}
