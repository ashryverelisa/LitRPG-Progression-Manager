using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ProgressionManager.Converters;

public class ValidityToColorConverter : IValueConverter
{
    public static readonly ValidityToColorConverter Instance = new();

    private static readonly IBrush ValidBrush = SolidColorBrush.Parse("#2ECC71");
    private static readonly IBrush InvalidBrush = SolidColorBrush.Parse("#E74C3C");

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