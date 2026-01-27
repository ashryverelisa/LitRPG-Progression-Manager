using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ProgressionManager.Converters;

public class ValidityToBackgroundConverter : IValueConverter
{
    public static readonly ValidityToBackgroundConverter Instance = new();

    private static readonly IBrush ValidBrush = SolidColorBrush.Parse("#1A3D2E");
    private static readonly IBrush InvalidBrush = SolidColorBrush.Parse("#3D1A1A");

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isValid)
        {
            return isValid ? ValidBrush : InvalidBrush;
        }
        return ValidBrush;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}