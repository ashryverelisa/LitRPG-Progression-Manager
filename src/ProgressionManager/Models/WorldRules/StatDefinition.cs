using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Models.WorldRules;

public partial class StatDefinition : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

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

    // Validation state
    [ObservableProperty]
    private bool _isFormulaValid = true;

    [ObservableProperty]
    private string? _formulaValidationError;

    [ObservableProperty]
    private double? _sampleValue;

    // Convenience
    public bool IsDerived => !string.IsNullOrWhiteSpace(Formula);

    /// <summary>
    /// Creates a copy of this stat definition.
    /// </summary>
    public StatDefinition Clone() => new()
    {
        Name = Name,
        Description = Description,
        BaseValue = BaseValue,
        GrowthPerLevel = GrowthPerLevel,
        MinValue = MinValue,
        MaxValue = MaxValue,
        Formula = Formula,
        IsFormulaValid = IsFormulaValid,
        FormulaValidationError = FormulaValidationError,
        SampleValue = SampleValue
    };
}
