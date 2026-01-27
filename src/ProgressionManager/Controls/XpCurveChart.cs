using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ProgressionManager.Controls.Charting;
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

    private static readonly SKColor _xpRequiredColor = new(124, 77, 255); // Purple
    private static readonly SKColor _totalXpColor = new(45, 212, 191);    // Teal
    private readonly LineChartRenderer _chartRenderer = new();

    static XpCurveChart()
    {
        AffectsRender<XpCurveChart>(LevelPreviewsProperty, ShowTotalXpProperty, ShowXpRequiredProperty);
    }

    public XpCurveChart()
    {
        LevelPreviewsProperty.Changed.AddClassHandler<XpCurveChart>(OnLevelPreviewsChanged);
    }

    private static void OnLevelPreviewsChanged(XpCurveChart chart, AvaloniaPropertyChangedEventArgs e)
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
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = new Rect(0, 0, Bounds.Width, Bounds.Height);
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var info = new SKImageInfo((int)bounds.Width, (int)bounds.Height);
        using var surface = SKSurface.Create(info);

        if (surface == null)
            return;

        var dataSeries = BuildDataSeries();
        _chartRenderer.Render(surface.Canvas, info, dataSeries, xAxisLabel: "Level");

        RenderToAvalonia(context, surface, bounds);
    }

    private List<ChartDataSeries> BuildDataSeries()
    {
        var series = new List<ChartDataSeries>();
        var previews = LevelPreviews?.ToList();

        if (previews == null || previews.Count == 0)
            return series;

        if (ShowTotalXp)
        {
            series.Add(new ChartDataSeries
            {
                Label = "Total XP",
                Color = _totalXpColor,
                Points = previews.Select(p => new ChartDataPoint(p.Level, p.TotalXp)).ToList()
            });
        }

        if (ShowXpRequired)
        {
            series.Add(new ChartDataSeries
            {
                Label = "XP per Level",
                Color = _xpRequiredColor,
                Points = previews.Select(p => new ChartDataPoint(p.Level, p.XpRequired)).ToList()
            });
        }

        return series;
    }

    private static void RenderToAvalonia(DrawingContext context, SKSurface surface, Rect bounds)
    {
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = data.AsStream();
        var bitmap = new Avalonia.Media.Imaging.Bitmap(stream);

        context.DrawImage(bitmap, bounds);
    }
}
