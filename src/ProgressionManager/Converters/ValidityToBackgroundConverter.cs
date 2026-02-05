using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ProgressionManager.Converters;

public class ValidityToBackgroundConverter : IValueConverter
{
    private static readonly IBrush _validBrush = SolidColorBrush.Parse("#1A3D2E");
    private static readonly IBrush _invalidBrush = SolidColorBrush.Parse("#3D1A1A");

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isValid)
        {
            return isValid ? _validBrush : _invalidBrush;
        }
        return _validBrush;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}