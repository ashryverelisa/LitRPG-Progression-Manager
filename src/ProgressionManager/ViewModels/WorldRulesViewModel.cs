using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgressionManager.Models.WorldRules;
using ProgressionManager.Services;

namespace ProgressionManager.ViewModels;

public partial class WorldRulesViewModel : ViewModelBase
{
    private readonly FormulaValidator _formulaValidator = new();

    // Stats
    public ObservableCollection<StatDefinition> Stats { get; } = new();

    [ObservableProperty]
    private StatDefinition? _selectedStat;

    [ObservableProperty]
    private bool _isEditingNewStat;

    // XP Curve
    [ObservableProperty]
    private XpCurveDefinition _xpCurve = new();

    [ObservableProperty]
    private XpCurveType _selectedCurveType = XpCurveType.Exponential;

    // Level-Up Rules
    [ObservableProperty]
    private LevelUpRules _levelUpRules = new();

    // Validation preview level (for testing formulas)
    [ObservableProperty]
    private int _previewLevel = 10;

    // Available curve types for binding
    public IEnumerable<XpCurveType> CurveTypes => Enum.GetValues<XpCurveType>();

    public WorldRulesViewModel()
    {
        SeedDefaultStats();
        ValidateAllFormulas();
        UpdateXpCurvePreviews();

        // Subscribe to XpCurve property changes
        XpCurve.PropertyChanged += OnXpCurvePropertyChanged;
    }

    private void OnXpCurvePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(XpCurveDefinition.Formula) or nameof(XpCurveDefinition.BaseXp)
            or nameof(XpCurveDefinition.LinearMultiplier) or nameof(XpCurveDefinition.ExponentialBase))
        {
            ValidateXpFormula();
            UpdateXpCurvePreviews();
        }
    }

    private void SeedDefaultStats()
    {
        Stats.Add(new StatDefinition
        {
            Name = "STR",
            Description = "Physical strength, affects melee damage",
            BaseValue = 10,
            GrowthPerLevel = 2,
            MinValue = 1,
            MaxValue = 999
        });

        Stats.Add(new StatDefinition
        {
            Name = "VIT",
            Description = "Vitality, affects HP and stamina",
            BaseValue = 8,
            GrowthPerLevel = 3,
            MinValue = 1,
            MaxValue = 999
        });

        Stats.Add(new StatDefinition
        {
            Name = "INT",
            Description = "Intelligence, affects mana and magic damage",
            BaseValue = 12,
            GrowthPerLevel = 2,
            MinValue = 1,
            MaxValue = 999
        });

        Stats.Add(new StatDefinition
        {
            Name = "AGI",
            Description = "Agility, affects speed and evasion",
            BaseValue = 10,
            GrowthPerLevel = 2,
            MinValue = 1,
            MaxValue = 999
        });

        Stats.Add(new StatDefinition
        {
            Name = "HP",
            Description = "Health Points - derived from VIT",
            Formula = "VIT * 12 + Level * 5",
            MinValue = 1
        });

        Stats.Add(new StatDefinition
        {
            Name = "Mana",
            Description = "Magical energy - derived from INT",
            Formula = "INT * 10 + Level * 3",
            MinValue = 0
        });

        Stats.Add(new StatDefinition
        {
            Name = "PhysDmg",
            Description = "Physical damage - derived from STR",
            Formula = "STR * 2 + Level",
            MinValue = 1
        });
    }

    #region Stat Management Commands

    [RelayCommand]
    private void AddBaseStat()
    {
        var newStat = new StatDefinition
        {
            Name = "NEW_STAT",
            Description = "New base stat",
            BaseValue = 10,
            GrowthPerLevel = 1,
            MinValue = 1,
            MaxValue = 999
        };
        Stats.Add(newStat);
        SelectedStat = newStat;
        IsEditingNewStat = true;
    }

    [RelayCommand]
    private void AddDerivedStat()
    {
        var newStat = new StatDefinition
        {
            Name = "NEW_DERIVED",
            Description = "New derived stat",
            Formula = "STR + INT",
            MinValue = 0
        };
        Stats.Add(newStat);
        SelectedStat = newStat;
        IsEditingNewStat = true;
        ValidateStatFormula(newStat);
    }

    [RelayCommand]
    private void DeleteStat(StatDefinition? stat)
    {
        if (stat == null) return;

        var dependentStats = Stats
            .Where(s => s.Formula != null && s.Formula.Contains(stat.Name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (dependentStats.Count > 0)
        {
            foreach (var dependent in dependentStats)
            {
                dependent.IsFormulaValid = false;
                dependent.FormulaValidationError = $"References deleted stat: {stat.Name}";
            }
        }

        Stats.Remove(stat);
        if (SelectedStat == stat)
        {
            SelectedStat = Stats.FirstOrDefault();
        }
    }

    [RelayCommand]
    private void DuplicateStat(StatDefinition? stat)
    {
        if (stat == null) return;

        var duplicate = stat.Clone();
        duplicate.Name = $"{stat.Name}_Copy";
        Stats.Add(duplicate);
        SelectedStat = duplicate;
    }

    [RelayCommand]
    private void MoveStatUp(StatDefinition? stat)
    {
        if (stat == null) return;
        var index = Stats.IndexOf(stat);
        if (index > 0)
        {
            Stats.Move(index, index - 1);
        }
    }

    [RelayCommand]
    private void MoveStatDown(StatDefinition? stat)
    {
        if (stat == null) return;
        var index = Stats.IndexOf(stat);
        if (index < Stats.Count - 1)
        {
            Stats.Move(index, index + 1);
        }
    }

    #endregion

    #region Formula Validation

    [RelayCommand]
    private void ValidateFormula(StatDefinition? stat)
    {
        if (stat != null)
        {
            ValidateStatFormula(stat);
        }
    }

    [RelayCommand]
    private void ValidateAllFormulas()
    {
        foreach (var stat in Stats.Where(s => s.IsDerived))
        {
            ValidateStatFormula(stat);
        }

        ValidateXpFormula();
    }

    private void ValidateStatFormula(StatDefinition stat)
    {
        if (string.IsNullOrWhiteSpace(stat.Formula))
        {
            stat.IsFormulaValid = true;
            stat.FormulaValidationError = null;
            stat.SampleValue = null;
            return;
        }

        var knownVariables = Stats.Select(s => s.Name).ToList();

        var testValues = new Dictionary<string, double>();
        foreach (var s in Stats.Where(s => !s.IsDerived))
        {
            var calculatedValue = (s.BaseValue ?? 10) + (s.GrowthPerLevel ?? 0) * PreviewLevel;
            testValues[s.Name] = calculatedValue;
        }
        testValues["Level"] = PreviewLevel;
        testValues["BaseValue"] = stat.BaseValue ?? 0;

        var result = _formulaValidator.Validate(stat.Formula, knownVariables, testValues);

        stat.IsFormulaValid = result.IsValid;
        stat.FormulaValidationError = result.ErrorMessage;
        stat.SampleValue = result.SampleResult;
    }

    [RelayCommand]
    private void ValidateXpFormula()
    {
        if (XpCurve.CurveType != XpCurveType.CustomFormula)
        {
            XpCurve.IsFormulaValid = true;
            XpCurve.FormulaValidationError = null;
            return;
        }

        var testValues = new Dictionary<string, double>
        {
            ["Level"] = PreviewLevel,
            ["BaseXP"] = XpCurve.BaseXp
        };

        var result = _formulaValidator.Validate(
            XpCurve.Formula,
            new[] { "Level", "BaseXP" },
            testValues);

        XpCurve.IsFormulaValid = result.IsValid;
        XpCurve.FormulaValidationError = result.ErrorMessage;
    }

    partial void OnPreviewLevelChanged(int value)
    {
        ValidateAllFormulas();
        UpdateXpCurvePreviews();
    }

    partial void OnSelectedStatChanged(StatDefinition? oldValue, StatDefinition? newValue)
    {
        if (oldValue != null)
        {
            oldValue.PropertyChanged -= OnSelectedStatPropertyChanged;
        }

        if (newValue != null)
        {
            newValue.PropertyChanged += OnSelectedStatPropertyChanged;
            if (newValue.IsDerived)
            {
                ValidateStatFormula(newValue);
            }
        }
    }

    private void OnSelectedStatPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is StatDefinition stat && e.PropertyName == nameof(StatDefinition.Formula))
        {
            ValidateStatFormula(stat);
        }
    }

    partial void OnSelectedCurveTypeChanged(XpCurveType value)
    {
        XpCurve.CurveType = value;
        ValidateXpFormula();
        UpdateXpCurvePreviews();
    }

    #endregion

    #region XP Curve Preview

    [RelayCommand]
    private void UpdateXpCurvePreviews()
    {
        XpCurve.LevelPreviews.Clear();

        long totalXp = 0;
        for (int level = 1; level <= 20; level++)
        {
            var xpRequired = CalculateXpForLevel(level);
            totalXp += xpRequired;

            XpCurve.LevelPreviews.Add(new XpLevelPreview
            {
                Level = level,
                XpRequired = xpRequired,
                TotalXp = totalXp
            });
        }
    }

    private long CalculateXpForLevel(int level)
    {
        return XpCurve.CurveType switch
        {
            XpCurveType.Linear => (long)(XpCurve.BaseXp + (level - 1) * XpCurve.LinearMultiplier),

            XpCurveType.Exponential => (long)(XpCurve.BaseXp * Math.Pow(XpCurve.ExponentialBase, level - 1)),

            XpCurveType.CustomFormula => CalculateCustomXp(level),

            _ => level * 100
        };
    }

    private long CalculateCustomXp(int level)
    {
        try
        {
            var values = new Dictionary<string, double>
            {
                ["Level"] = level,
                ["BaseXP"] = XpCurve.BaseXp
            };

            return (long)_formulaValidator.Evaluate(XpCurve.Formula, values);
        }
        catch
        {
            return level * 100; // Fallback
        }
    }

    #endregion

    #region Preview Calculations

    public double GetStatPreviewValue(StatDefinition stat)
    {
        if (!stat.IsDerived)
        {
            return (stat.BaseValue ?? 0) + (stat.GrowthPerLevel ?? 0) * PreviewLevel;
        }

        if (!stat.IsFormulaValid || string.IsNullOrWhiteSpace(stat.Formula))
        {
            return 0;
        }

        try
        {
            var values = BuildStatValuesDictionary();
            return _formulaValidator.Evaluate(stat.Formula, values);
        }
        catch
        {
            return 0;
        }
    }

    private Dictionary<string, double> BuildStatValuesDictionary()
    {
        var values = new Dictionary<string, double>
        {
            ["Level"] = PreviewLevel
        };

        foreach (var stat in Stats.Where(s => !s.IsDerived))
        {
            values[stat.Name] = (stat.BaseValue ?? 0) + (stat.GrowthPerLevel ?? 0) * PreviewLevel;
        }

        foreach (var stat in Stats.Where(s => s.IsDerived && s.IsFormulaValid))
        {
            try
            {
                values[stat.Name] = _formulaValidator.Evaluate(stat.Formula!, values);
            }
            catch
            {
                values[stat.Name] = 0;
            }
        }

        return values;
    }

    #endregion

    #region Import/Export

    [RelayCommand]
    private void ResetToDefaults()
    {
        Stats.Clear();
        SeedDefaultStats();
        ValidateAllFormulas();

        XpCurve = new XpCurveDefinition();
        SelectedCurveType = XpCurveType.Exponential;
        UpdateXpCurvePreviews();

        LevelUpRules = new LevelUpRules();
    }

    #endregion
}