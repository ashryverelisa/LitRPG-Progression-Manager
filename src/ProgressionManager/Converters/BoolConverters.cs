using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ProgressionManager.Converters;

public class BoolToValueConverter : IValueConverter
{
    public static readonly BoolToValueConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue || parameter is not string paramString)
            return null;

        var parts = paramString.Split(';');
        if (parts.Length != 2)
            return null;

        var result = boolValue ? parts[0] : parts[1];

        if (targetType == typeof(IBrush) || targetType == typeof(ISolidColorBrush))
        {
            if (result.StartsWith('#'))
            {
                return SolidColorBrush.Parse(result);
            }
        }

        return result;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

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

public class ValidityToStatusTextConverter : IValueConverter
{
    public static readonly ValidityToStatusTextConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isValid)
        {
            var format = parameter as string ?? "✓ Valid;✗ Invalid";
            var parts = format.Split(';');
            return isValid ? parts[0] : (parts.Length > 1 ? parts[1] : "Invalid");
        }
        return "✓ Valid";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
