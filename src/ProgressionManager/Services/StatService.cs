using System;
using System.Collections.Generic;
using System.Linq;
using ProgressionManager.Models.WorldRules;
using ProgressionManager.Services.Interfaces;

namespace ProgressionManager.Services;

public class StatService(IFormulaValidatorService formulaValidator) : IStatService
{
    public IEnumerable<StatDefinition> GetDefaultStats()
    {
        return
        [
            new StatDefinition
            {
                Name = "STR",
                Description = "Physical strength, affects melee damage",
                BaseValue = 10,
                GrowthPerLevel = 2,
                MinValue = 1,
                MaxValue = 999
            },
            new StatDefinition
            {
                Name = "VIT",
                Description = "Vitality, affects HP and stamina",
                BaseValue = 8,
                GrowthPerLevel = 3,
                MinValue = 1,
                MaxValue = 999
            },
            new StatDefinition
            {
                Name = "INT",
                Description = "Intelligence, affects mana and magic damage",
                BaseValue = 12,
                GrowthPerLevel = 2,
                MinValue = 1,
                MaxValue = 999
            },
            new StatDefinition
            {
                Name = "AGI",
                Description = "Agility, affects speed and evasion",
                BaseValue = 10,
                GrowthPerLevel = 2,
                MinValue = 1,
                MaxValue = 999
            },
            new StatDefinition
            {
                Name = "HP",
                Description = "Health Points - derived from VIT",
                Formula = "VIT * 12 + Level * 5",
                MinValue = 1
            },
            new StatDefinition
            {
                Name = "Mana",
                Description = "Magical energy - derived from INT",
                Formula = "INT * 10 + Level * 3",
                MinValue = 0
            },
            new StatDefinition
            {
                Name = "PhysDmg",
                Description = "Physical damage - derived from STR",
                Formula = "STR * 2 + Level",
                MinValue = 1
            }
        ];
    }

    public StatDefinition CreateBaseStat(string name = "NEW_STAT")
    {
        return new StatDefinition
        {
            Name = name,
            Description = "New base stat",
            BaseValue = 10,
            GrowthPerLevel = 1,
            MinValue = 1,
            MaxValue = 999
        };
    }

    public StatDefinition CreateDerivedStat(string name = "NEW_DERIVED")
    {
        return new StatDefinition
        {
            Name = name,
            Description = "New derived stat",
            Formula = "STR + INT",
            MinValue = 0
        };
    }

    public StatDefinition CloneStat(StatDefinition stat)
    {
        var clone = stat.Clone();
        clone.Name = $"{stat.Name}_Copy";
        return clone;
    }

    public IEnumerable<StatDefinition> FindDependentStats(StatDefinition stat, IEnumerable<StatDefinition> allStats)
    {
        return allStats
            .Where(s => s.Formula != null && s.Formula.Contains(stat.Name, StringComparison.OrdinalIgnoreCase));
    }

    public void ValidateStatFormula(StatDefinition stat, IEnumerable<StatDefinition> allStats, int previewLevel)
    {
        if (string.IsNullOrWhiteSpace(stat.Formula))
        {
            stat.IsFormulaValid = true;
            stat.FormulaValidationError = null;
            stat.SampleValue = null;
            return;
        }

        var statsList = allStats.ToList();
        var knownVariables = statsList.Select(s => s.Name).ToList();

        var testValues = new Dictionary<string, double>();
        foreach (var s in statsList.Where(s => !s.IsDerived))
        {
            var calculatedValue = (s.BaseValue ?? 10) + (s.GrowthPerLevel ?? 0) * previewLevel;
            testValues[s.Name] = calculatedValue;
        }
        testValues["Level"] = previewLevel;
        testValues["BaseValue"] = stat.BaseValue ?? 0;

        var result = formulaValidator.Validate(stat.Formula, knownVariables, testValues);

        stat.IsFormulaValid = result.IsValid;
        stat.FormulaValidationError = result.ErrorMessage;
        stat.SampleValue = result.SampleResult;
    }
}
