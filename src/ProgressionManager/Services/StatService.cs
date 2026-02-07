﻿using System;
using System.Collections.Generic;
using System.Linq;
using ProgressionManager.Models.WorldRules;
using ProgressionManager.Services.Interfaces;

namespace ProgressionManager.Services;

public class StatService(
    IFormulaValidatorService formulaValidator,
    IEmbeddedResourceService resourceService) : IStatService
{
    public IEnumerable<StatDefinition> GetDefaultStats()
    {
        var stats = resourceService.LoadResourceAsJson<List<StatDefinition>>("DefaultStats.json");
        return stats ?? [];
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
