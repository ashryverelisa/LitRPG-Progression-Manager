using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using ProgressionManager.Models.WorldRules;
using SkiaSharp;

namespace ProgressionManager.Controls;

public class XpCurveChart : Control
{
    public static readonly StyledProperty<ObservableCollection<XpLevelPreview>?> LevelPreviewsProperty =
        AvaloniaProperty.Register<XpCurveChart, ObservableCollection<XpLevelPreview>?>(nameof(LevelPreviews));

    public static readonly StyledProperty<bool> ShowTotalXpProperty =
        AvaloniaProperty.Register<XpCurveChart, bool>(nameof(ShowTotalXp), true);

    public static readonly StyledProperty<bool> ShowXpRequiredProperty =
        AvaloniaProperty.Register<XpCurveChart, bool>(nameof(ShowXpRequired), true);

    public ObservableCollection<XpLevelPreview>? LevelPreviews
    {
        get => GetValue(LevelPreviewsProperty);
        set => SetValue(LevelPreviewsProperty, value);
    }

    public bool ShowTotalXp
    {
        get => GetValue(ShowTotalXpProperty);
        set => SetValue(ShowTotalXpProperty, value);
    }

    public bool ShowXpRequired
    {
        get => GetValue(ShowXpRequiredProperty);
        set => SetValue(ShowXpRequiredProperty, value);
    }

    // Chart colors matching the dark theme
    private static readonly SKColor BackgroundColor = new(10, 12, 16);
    private static readonly SKColor GridColor = new(42, 48, 66);
    private static readonly SKColor TextColor = new(192, 202, 245);
    private static readonly SKColor XpRequiredColor = new(124, 77, 255); // Purple
    private static readonly SKColor TotalXpColor = new(45, 212, 191);   // Teal
    private static readonly SKColor AxisColor = new(65, 72, 104);

    private const float Padding = 50f;
    private const float RightPadding = 20f;
    private const float TopPadding = 20f;
    private const float BottomPadding = 40f;

    static XpCurveChart()
    {
        AffectsRender<XpCurveChart>(LevelPreviewsProperty, ShowTotalXpProperty, ShowXpRequiredProperty);
    }

    public XpCurveChart()
    {
        // Subscribe to collection changes
        LevelPreviewsProperty.Changed.AddClassHandler<XpCurveChart>((chart, e) =>
        {
            if (e.OldValue is ObservableCollection<XpLevelPreview> oldCollection)
            {
                oldCollection.CollectionChanged -= chart.OnCollectionChanged;
            }
            if (e.NewValue is ObservableCollection<XpLevelPreview> newCollection)
            {
                newCollection.CollectionChanged += chart.OnCollectionChanged;
            }
            chart.InvalidateVisual();
        });
    }

    private void OnCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = new Rect(0, 0, Bounds.Width, Bounds.Height);

        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        // Create SkiaSharp surface
        var info = new SKImageInfo((int)bounds.Width, (int)bounds.Height);
        using var surface = SKSurface.Create(info);

        if (surface == null)
            return;

        var canvas = surface.Canvas;
        canvas.Clear(BackgroundColor);

        var previews = LevelPreviews?.ToList();
        if (previews == null || previews.Count == 0)
        {
            DrawNoDataMessage(canvas, info);
        }
        else
        {
            DrawChart(canvas, info, previews);
        }

        // Render the SkiaSharp surface to Avalonia
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = data.AsStream();
        var bitmap = new Avalonia.Media.Imaging.Bitmap(stream);

        context.DrawImage(bitmap, bounds);
    }

    private void DrawNoDataMessage(SKCanvas canvas, SKImageInfo info)
    {
        using var paint = new SKPaint
        {
            Color = TextColor,
            TextSize = 14,
            IsAntialias = true,
            TextAlign = SKTextAlign.Center
        };

        canvas.DrawText("No XP data to display", info.Width / 2f, info.Height / 2f, paint);
    }

    private void DrawChart(SKCanvas canvas, SKImageInfo info, System.Collections.Generic.List<XpLevelPreview> previews)
    {
        var chartArea = new SKRect(
            Padding,
            TopPadding,
            info.Width - RightPadding,
            info.Height - BottomPadding
        );

        // Calculate data ranges
        var maxLevel = previews.Max(p => p.Level);
        var maxXpRequired = ShowXpRequired ? previews.Max(p => p.XpRequired) : 0;
        var maxTotalXp = ShowTotalXp ? previews.Max(p => p.TotalXp) : 0;
        var maxValue = Math.Max(maxXpRequired, maxTotalXp);

        if (maxValue == 0) maxValue = 1;
        if (maxLevel == 0) maxLevel = 1;

        // Draw grid and axes
        DrawGrid(canvas, chartArea, maxLevel, maxValue);
        DrawAxes(canvas, chartArea, maxLevel, maxValue);

        // Draw data lines
        if (ShowXpRequired)
        {
            DrawDataLine(canvas, chartArea, previews, p => p.XpRequired, maxLevel, maxValue, XpRequiredColor, "XP per Level");
        }

        if (ShowTotalXp)
        {
            DrawDataLine(canvas, chartArea, previews, p => p.TotalXp, maxLevel, maxValue, TotalXpColor, "Total XP");
        }

        // Draw legend
        DrawLegend(canvas, info);
    }

    private void DrawGrid(SKCanvas canvas, SKRect chartArea, int maxLevel, long maxValue)
    {
        using var gridPaint = new SKPaint
        {
            Color = GridColor,
            StrokeWidth = 1,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        // Horizontal grid lines (5 lines)
        for (int i = 0; i <= 4; i++)
        {
            var y = chartArea.Top + (chartArea.Height * i / 4f);
            canvas.DrawLine(chartArea.Left, y, chartArea.Right, y, gridPaint);
        }

        // Vertical grid lines
        var gridStepX = Math.Max(1, maxLevel / 5);
        for (int level = gridStepX; level <= maxLevel; level += gridStepX)
        {
            var x = chartArea.Left + (chartArea.Width * level / maxLevel);
            canvas.DrawLine(x, chartArea.Top, x, chartArea.Bottom, gridPaint);
        }
    }

    private void DrawAxes(SKCanvas canvas, SKRect chartArea, int maxLevel, long maxValue)
    {
        using var axisPaint = new SKPaint
        {
            Color = AxisColor,
            StrokeWidth = 2,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        using var textPaint = new SKPaint
        {
            Color = TextColor,
            TextSize = 11,
            IsAntialias = true
        };

        // Draw axes
        canvas.DrawLine(chartArea.Left, chartArea.Bottom, chartArea.Right, chartArea.Bottom, axisPaint);
        canvas.DrawLine(chartArea.Left, chartArea.Top, chartArea.Left, chartArea.Bottom, axisPaint);

        // X-axis labels (levels)
        var labelStepX = Math.Max(1, maxLevel / 5);
        textPaint.TextAlign = SKTextAlign.Center;
        for (int level = 0; level <= maxLevel; level += labelStepX)
        {
            if (level == 0) continue;
            var x = chartArea.Left + (chartArea.Width * level / maxLevel);
            canvas.DrawText(level.ToString(), x, chartArea.Bottom + 20, textPaint);
        }

        // X-axis title
        canvas.DrawText("Level", chartArea.Left + chartArea.Width / 2, chartArea.Bottom + 35, textPaint);

        // Y-axis labels
        textPaint.TextAlign = SKTextAlign.Right;
        for (int i = 0; i <= 4; i++)
        {
            var y = chartArea.Bottom - (chartArea.Height * i / 4f);
            var value = maxValue * i / 4;
            canvas.DrawText(FormatNumber(value), chartArea.Left - 5, y + 4, textPaint);
        }
    }

    private void DrawDataLine(SKCanvas canvas, SKRect chartArea,
        System.Collections.Generic.List<XpLevelPreview> previews,
        Func<XpLevelPreview, long> valueSelector,
        int maxLevel, long maxValue, SKColor color, string label)
    {
        if (previews.Count == 0) return;

        using var linePaint = new SKPaint
        {
            Color = color,
            StrokeWidth = 3,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        using var fillPaint = new SKPaint
        {
            Color = color.WithAlpha(40),
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        using var pointPaint = new SKPaint
        {
            Color = color,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        var path = new SKPath();
        var fillPath = new SKPath();

        var firstPoint = true;
        SKPoint lastPoint = default;

        foreach (var preview in previews)
        {
            var x = chartArea.Left + (chartArea.Width * preview.Level / maxLevel);
            var y = chartArea.Bottom - (chartArea.Height * valueSelector(preview) / maxValue);

            if (firstPoint)
            {
                path.MoveTo(x, y);
                fillPath.MoveTo(chartArea.Left, chartArea.Bottom);
                fillPath.LineTo(x, y);
                firstPoint = false;
            }
            else
            {
                path.LineTo(x, y);
                fillPath.LineTo(x, y);
            }

            lastPoint = new SKPoint(x, y);

            // Draw data point
            canvas.DrawCircle(x, y, 4, pointPaint);
        }

        // Complete fill path
        fillPath.LineTo(lastPoint.X, chartArea.Bottom);
        fillPath.Close();

        // Draw fill and line
        canvas.DrawPath(fillPath, fillPaint);
        canvas.DrawPath(path, linePaint);
    }

    private void DrawLegend(SKCanvas canvas, SKImageInfo info)
    {
        using var textPaint = new SKPaint
        {
            Color = TextColor,
            TextSize = 12,
            IsAntialias = true
        };

        var legendY = 12f;
        var legendX = info.Width - RightPadding - 10;

        if (ShowTotalXp)
        {
            using var colorPaint = new SKPaint { Color = TotalXpColor, Style = SKPaintStyle.Fill };
            textPaint.TextAlign = SKTextAlign.Right;
            canvas.DrawText("Total XP", legendX - 15, legendY + 4, textPaint);
            canvas.DrawCircle(legendX - 5, legendY, 5, colorPaint);
            legendY += 18;
        }

        if (ShowXpRequired)
        {
            using var colorPaint = new SKPaint { Color = XpRequiredColor, Style = SKPaintStyle.Fill };
            textPaint.TextAlign = SKTextAlign.Right;
            canvas.DrawText("XP per Level", legendX - 15, legendY + 4, textPaint);
            canvas.DrawCircle(legendX - 5, legendY, 5, colorPaint);
        }
    }

    private static string FormatNumber(long value)
    {
        return value switch
        {
            >= 1_000_000_000 => $"{value / 1_000_000_000.0:F1}B",
            >= 1_000_000 => $"{value / 1_000_000.0:F1}M",
            >= 1_000 => $"{value / 1_000.0:F1}K",
            _ => value.ToString()
        };
    }
}
