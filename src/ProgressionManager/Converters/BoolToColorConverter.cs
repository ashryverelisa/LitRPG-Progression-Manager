using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ProgressionManager.Converters;

/// <summary>
/// Converts a boolean value to a color. Configurable true/false colors.
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    /// <summary>
    /// The color to use when the value is true.
    /// </summary>
    public string TrueColor { get; set; } = "#2ECC71";

    /// <summary>
    /// The color to use when the value is false.
    /// </summary>
    public string FalseColor { get; set; } = "#E74C3C";

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            var colorStr = boolValue ? TrueColor : FalseColor;
            return SolidColorBrush.Parse(colorStr);
        }
        return SolidColorBrush.Parse(TrueColor);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


