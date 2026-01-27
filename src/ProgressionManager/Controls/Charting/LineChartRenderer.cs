using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace ProgressionManager.Controls.Charting;

public class LineChartRenderer(ChartTheme? theme = null, ChartPadding? padding = null)
{
    private readonly ChartTheme _theme = theme ?? ChartTheme.Dark;
    private readonly ChartPadding _padding = padding ?? ChartPadding.Default;

    private const int GridDivisions = 4;
    private const int AxisLabelDivisions = 5;

    public void Render(
        SKCanvas canvas,
        SKImageInfo info,
        IReadOnlyList<ChartDataSeries> dataSeries,
        string xAxisLabel = "X",
        string? yAxisLabel = null)
    {
        canvas.Clear(_theme.BackgroundColor);

        if (dataSeries.Count == 0 || dataSeries.All(s => s.Points.Count == 0))
        {
            DrawNoDataMessage(canvas, info);
            return;
        }

        var chartArea = CalculateChartArea(info);
        var (maxX, maxY) = CalculateDataRanges(dataSeries);

        DrawGrid(canvas, chartArea, maxX, maxY);
        DrawAxes(canvas, chartArea, maxX, maxY, xAxisLabel, yAxisLabel);

        foreach (var series in dataSeries)
        {
            DrawDataSeries(canvas, chartArea, series, maxX, maxY);
        }

        DrawLegend(canvas, info, dataSeries);
    }

    private SKRect CalculateChartArea(SKImageInfo info)
    {
        return new SKRect(
            _padding.Left,
            _padding.Top,
            info.Width - _padding.Right,
            info.Height - _padding.Bottom
        );
    }

    private static (float maxX, float maxY) CalculateDataRanges(IReadOnlyList<ChartDataSeries> dataSeries)
    {
        var maxX = dataSeries.SelectMany(s => s.Points).Max(p => p.X);
        var maxY = dataSeries.SelectMany(s => s.Points).Max(p => p.Y);

        if (maxX <= 0) maxX = 1;
        if (maxY <= 0) maxY = 1;

        return (maxX, maxY);
    }

    private void DrawNoDataMessage(SKCanvas canvas, SKImageInfo info)
    {
        using var paint = ChartPaintFactory.CreateTextPaint(_theme, _theme.NoDataTextSize, SKTextAlign.Center);
        canvas.DrawText("No data to display", info.Width / 2f, info.Height / 2f, paint);
    }

    private void DrawGrid(SKCanvas canvas, SKRect chartArea, float maxX, float _)
    {
        using var gridPaint = ChartPaintFactory.CreateGridPaint(_theme);

        for (var i = 0; i <= GridDivisions; i++)
        {
            var y = chartArea.Top + (chartArea.Height * i / GridDivisions);
            canvas.DrawLine(chartArea.Left, y, chartArea.Right, y, gridPaint);
        }

        var gridStepX = Math.Max(1, (int)(maxX / AxisLabelDivisions));
        for (var level = gridStepX; level <= maxX; level += gridStepX)
        {
            var x = chartArea.Left + (chartArea.Width * level / maxX);
            canvas.DrawLine(x, chartArea.Top, x, chartArea.Bottom, gridPaint);
        }
    }

    private void DrawAxes(SKCanvas canvas, SKRect chartArea, float maxX, float maxY, string xAxisLabel, string? _ = null)
    {
        using var axisPaint = ChartPaintFactory.CreateAxisPaint(_theme);
        using var textPaint = ChartPaintFactory.CreateTextPaint(_theme, _theme.AxisLabelTextSize);

        canvas.DrawLine(chartArea.Left, chartArea.Bottom, chartArea.Right, chartArea.Bottom, axisPaint);
        canvas.DrawLine(chartArea.Left, chartArea.Top, chartArea.Left, chartArea.Bottom, axisPaint);

        DrawXAxisLabels(canvas, chartArea, maxX, textPaint, xAxisLabel);
        DrawYAxisLabels(canvas, chartArea, maxY, textPaint);
    }

    private void DrawXAxisLabels(SKCanvas canvas, SKRect chartArea, float maxX, SKPaint textPaint, string xAxisLabel)
    {
        textPaint.TextAlign = SKTextAlign.Center;
        var labelStepX = Math.Max(1, (int)(maxX / AxisLabelDivisions));

        for (var level = labelStepX; level <= maxX; level += labelStepX)
        {
            var x = chartArea.Left + (chartArea.Width * level / maxX);
            canvas.DrawText(level.ToString(), x, chartArea.Bottom + 20, textPaint);
        }

        // X-axis title
        canvas.DrawText(xAxisLabel, chartArea.Left + chartArea.Width / 2, chartArea.Bottom + 35, textPaint);
    }

    private void DrawYAxisLabels(SKCanvas canvas, SKRect chartArea, float maxY, SKPaint textPaint)
    {
        textPaint.TextAlign = SKTextAlign.Right;

        for (var i = 0; i <= GridDivisions; i++)
        {
            var y = chartArea.Bottom - (chartArea.Height * i / GridDivisions);
            var value = (long)(maxY * i / GridDivisions);
            canvas.DrawText(NumberFormatter.FormatCompact(value), chartArea.Left - 5, y + 4, textPaint);
        }
    }

    private void DrawDataSeries(SKCanvas canvas, SKRect chartArea, ChartDataSeries series, float maxX, float maxY)
    {
        if (series.Points.Count == 0) return;

        using var linePaint = ChartPaintFactory.CreateLinePaint(series.Color, _theme.DataLineWidth);
        using var fillPaint = ChartPaintFactory.CreateFillPaint(series.Color, _theme.FillAlpha);
        using var pointPaint = ChartPaintFactory.CreatePointPaint(series.Color);

        using var linePath = new SKPath();
        using var fillPath = new SKPath();

        var firstPoint = true;
        SKPoint lastPoint = default;

        foreach (var point in series.Points)
        {
            var screenX = chartArea.Left + (chartArea.Width * point.X / maxX);
            var screenY = chartArea.Bottom - (chartArea.Height * point.Y / maxY);

            if (firstPoint)
            {
                linePath.MoveTo(screenX, screenY);
                fillPath.MoveTo(chartArea.Left, chartArea.Bottom);
                fillPath.LineTo(screenX, screenY);
                firstPoint = false;
            }
            else
            {
                linePath.LineTo(screenX, screenY);
                fillPath.LineTo(screenX, screenY);
            }

            lastPoint = new SKPoint(screenX, screenY);

            if (series.ShowPoints)
            {
                canvas.DrawCircle(screenX, screenY, _theme.DataPointRadius, pointPaint);
            }
        }

        if (series.ShowFill)
        {
            fillPath.LineTo(lastPoint.X, chartArea.Bottom);
            fillPath.Close();
            canvas.DrawPath(fillPath, fillPaint);
        }

        canvas.DrawPath(linePath, linePaint);
    }

    private void DrawLegend(SKCanvas canvas, SKImageInfo info, IReadOnlyList<ChartDataSeries> dataSeries)
    {
        using var textPaint = ChartPaintFactory.CreateTextPaint(_theme, _theme.LegendTextSize, SKTextAlign.Right);

        var legendY = 12f;
        var legendX = info.Width - _padding.Right - 10;

        foreach (var series in dataSeries)
        {
            using var colorPaint = ChartPaintFactory.CreatePointPaint(series.Color);

            canvas.DrawText(series.Label, legendX - 15, legendY + 4, textPaint);
            canvas.DrawCircle(legendX - 5, legendY, _theme.LegendPointRadius, colorPaint);
            legendY += 18;
        }
    }
}
