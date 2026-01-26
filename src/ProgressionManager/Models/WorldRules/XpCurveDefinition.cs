using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Models.WorldRules;

public partial class XpCurveDefinition : ObservableObject
{
    [ObservableProperty]
    private XpCurveType _curveType;

    // Used when CurveType == CustomFormula
    [ObservableProperty]
    private string _formula = "Level * 100";

    // Optional tuning parameters
    [ObservableProperty]
    private double _linearMultiplier = 100;

    [ObservableProperty]
    private double _exponentialBase = 1.15;
}