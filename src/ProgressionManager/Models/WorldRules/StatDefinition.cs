using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Models.WorldRules;

public partial class StatDefinition : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    // For base stats (STR, INT, etc.)
    [ObservableProperty]
    private int? _baseValue;

    [ObservableProperty]
    private int? _growthPerLevel;

    [ObservableProperty]
    private int? _minValue;

    [ObservableProperty]
    private int? _maxValue;

    // For derived stats (HP, Mana, etc.)
    [ObservableProperty]
    private string? _formula;

    // Convenience
    public bool IsDerived => !string.IsNullOrWhiteSpace(Formula);
}
