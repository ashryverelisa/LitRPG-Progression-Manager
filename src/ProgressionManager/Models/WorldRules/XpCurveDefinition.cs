using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Models.WorldRules;

public partial class XpCurveDefinition : ObservableObject
{
    [ObservableProperty]
    private XpCurveType _curveType = XpCurveType.Exponential;

    // Used when CurveType == CustomFormula
    [ObservableProperty]
    private string _formula = "Level * 100";

    // Optional tuning parameters
    [ObservableProperty]
    private double _linearMultiplier = 100;

    [ObservableProperty]
    private double _exponentialBase = 1.15;

    [ObservableProperty]
    private int _baseXp = 100;

    // Validation state
    [ObservableProperty]
    private bool _isFormulaValid = true;

    [ObservableProperty]
    private string? _formulaValidationError;

    // Preview values for different levels
    public ObservableCollection<XpLevelPreview> LevelPreviews { get; } = new();
}

public partial class XpLevelPreview : ObservableObject
{
    [ObservableProperty]
    private int _level;

    [ObservableProperty]
    private long _xpRequired;

    [ObservableProperty]
    private long _totalXp;
}