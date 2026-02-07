using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Models.ClassesRaces;

/// <summary>
/// Represents a modifier to a base stat for a class template.
/// Positive values boost stats, negative values reduce them.
/// </summary>
public partial class StatModifier : ObservableObject
{
    /// <summary>
    /// The name/abbreviation of the stat being modified (e.g., "STR", "INT", "VIT").
    /// </summary>
    private string _statName = string.Empty;

    public string StatName
    {
        get => _statName;
        set
        {
            // Prevent null/empty values from binding timing issues during navigation
            // Note: Avalonia binding can pass null at runtime despite non-nullable type
            if (string.IsNullOrEmpty(value)) return;
            SetProperty(ref _statName, value);
        }
    }

    /// <summary>
    /// The modifier value. Positive for bonuses, negative for penalties.
    /// </summary>
    [ObservableProperty]
    private int _value;

    /// <summary>
    /// Optional color code for UI display.
    /// </summary>
    [ObservableProperty]
    private string? _color;

    /// <summary>
    /// Determines if this is a bonus (positive) or penalty (negative).
    /// </summary>
    public bool IsBonus => Value >= 0;

    /// <summary>
    /// Formatted display value with +/- prefix.
    /// </summary>
    public string DisplayValue => Value >= 0 ? $"+{Value}" : Value.ToString();

    /// <summary>
    /// Creates a copy of this stat modifier.
    /// </summary>
    public StatModifier Clone() => new()
    {
        StatName = StatName,
        Value = Value,
        Color = Color
    };
}

