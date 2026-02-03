using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgressionManager.Models.WorldRules;
using ProgressionManager.Services.Interfaces;

namespace ProgressionManager.ViewModels;

public partial class WorldRulesViewModel : ViewModelBase
{
    private readonly IStatService _statService = null!;
    private readonly IXpCurveCalculatorService _xpCurveCalculatorService = null!;

    public ObservableCollection<StatDefinition> Stats { get; } = [];
    public IEnumerable<XpCurveType> CurveTypes => Enum.GetValues<XpCurveType>();

    [ObservableProperty] private StatDefinition? _selectedStat;
    [ObservableProperty] private bool _isEditingNewStat;
    [ObservableProperty] private XpCurveDefinition _xpCurve = new();
    [ObservableProperty] private XpCurveType _selectedCurveType = XpCurveType.Exponential;
    [ObservableProperty] private LevelUpRules _levelUpRules = new();
    [ObservableProperty] private int _previewLevel = 10;

    public WorldRulesViewModel()
    {

    }

    public WorldRulesViewModel(IStatService statService, IXpCurveCalculatorService xpCurveCalculatorService)
    {
        _statService = statService;
        _xpCurveCalculatorService = xpCurveCalculatorService;

        InitializeStats();
        SubscribeToXpCurveChanges();
    }

    private void InitializeStats()
    {
        foreach (var stat in _statService.GetDefaultStats())
            Stats.Add(stat);

        ValidateAllFormulasCommand.Execute(null);
        UpdateXpCurvePreviewsCommand.Execute(null);
    }

    private void SubscribeToXpCurveChanges()
    {
        XpCurve.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(XpCurveDefinition.Formula) or nameof(XpCurveDefinition.BaseXp)
                or nameof(XpCurveDefinition.LinearMultiplier) or nameof(XpCurveDefinition.ExponentialBase))
            {
                RefreshXpCurve();
            }
        };
    }

    private void RefreshXpCurve()
    {
        _xpCurveCalculatorService.ValidateFormula(XpCurve, PreviewLevel);
        UpdateXpCurvePreviewsCommand.Execute(null);
    }

    [RelayCommand]
    private void AddBaseStat() => AddStat(_statService.CreateBaseStat());

    [RelayCommand]
    private void AddDerivedStat()
    {
        var stat = _statService.CreateDerivedStat();
        AddStat(stat);
        _statService.ValidateStatFormula(stat, Stats, PreviewLevel);
    }

    private void AddStat(StatDefinition stat)
    {
        Stats.Add(stat);
        SelectedStat = stat;
        IsEditingNewStat = true;
    }

    [RelayCommand]
    private void DeleteStat(StatDefinition? stat)
    {
        if (stat == null) return;

        InvalidateDependentStats(stat);
        Stats.Remove(stat);

        if (SelectedStat == stat)
            SelectedStat = Stats.FirstOrDefault();
    }

    private void InvalidateDependentStats(StatDefinition deletedStat)
    {
        foreach (var dependent in _statService.FindDependentStats(deletedStat, Stats))
        {
            dependent.IsFormulaValid = false;
            dependent.FormulaValidationError = $"References deleted stat: {deletedStat.Name}";
        }
    }

    [RelayCommand]
    private void DuplicateStat(StatDefinition? stat)
    {
        if (stat == null) return;
        var duplicate = _statService.CloneStat(stat);
        Stats.Add(duplicate);
        SelectedStat = duplicate;
    }

    [RelayCommand]
    private void MoveStatUp(StatDefinition? stat) => MoveStat(stat, -1);

    [RelayCommand]
    private void MoveStatDown(StatDefinition? stat) => MoveStat(stat, 1);

    private void MoveStat(StatDefinition? stat, int direction)
    {
        if (stat == null) return;
        var index = Stats.IndexOf(stat);
        var newIndex = index + direction;

        if (newIndex >= 0 && newIndex < Stats.Count)
            Stats.Move(index, newIndex);
    }

    [RelayCommand]
    private void ValidateFormula(StatDefinition? stat)
    {
        if (stat != null)
            _statService.ValidateStatFormula(stat, Stats, PreviewLevel);
    }

    [RelayCommand]
    private void ValidateAllFormulas()
    {
        foreach (var stat in Stats.Where(s => s.IsDerived))
            _statService.ValidateStatFormula(stat, Stats, PreviewLevel);

        _xpCurveCalculatorService.ValidateFormula(XpCurve, PreviewLevel);
    }

    partial void OnPreviewLevelChanged(int value)
    {
        ValidateAllFormulasCommand.Execute(null);
        UpdateXpCurvePreviewsCommand.Execute(null);
    }

    partial void OnSelectedCurveTypeChanged(XpCurveType value)
    {
        XpCurve.CurveType = value;
        RefreshXpCurve();
    }

    [RelayCommand]
    private void UpdateXpCurvePreviews()
    {
        XpCurve.LevelPreviews.Clear();
        foreach (var preview in _xpCurveCalculatorService.GeneratePreviews(XpCurve))
            XpCurve.LevelPreviews.Add(preview);
    }

    [RelayCommand]
    private void ResetToDefaults()
    {
        Stats.Clear();
        InitializeStats();

        XpCurve = new XpCurveDefinition();
        SubscribeToXpCurveChanges();
        SelectedCurveType = XpCurveType.Exponential;

        LevelUpRules = new LevelUpRules();
    }
}