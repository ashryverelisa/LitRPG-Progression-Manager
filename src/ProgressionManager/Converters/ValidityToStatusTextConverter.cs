using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ProgressionManager.Converters;

public class ValidityToStatusTextConverter : IValueConverter
{

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool isValid) return "✓ Valid";
        var format = parameter as string ?? "✓ Valid;✗ Invalid";
        var parts = format.Split(';');
        return isValid ? parts[0] : (parts.Length > 1 ? parts[1] : "Invalid");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}