using System;
using System.Collections.Generic;
using ProgressionManager.Models.WorldRules;
using ProgressionManager.Services.Interfaces;

namespace ProgressionManager.Services;

public class XpCurveCalculatorServiceService(IFormulaValidatorService formulaValidator) : IXpCurveCalculatorService
{
    public long CalculateXpForLevel(XpCurveDefinition curve, int level)
    {
        return curve.CurveType switch
        {
            XpCurveType.Linear => (long)(curve.BaseXp + (level - 1) * curve.LinearMultiplier),
            XpCurveType.Exponential => (long)(curve.BaseXp * Math.Pow(curve.ExponentialBase, level - 1)),
            XpCurveType.CustomFormula => CalculateCustomXp(curve, level),
            _ => level * 100
        };
    }

    public IEnumerable<XpLevelPreview> GeneratePreviews(XpCurveDefinition curve, int maxLevel = 20)
    {
        var previews = new List<XpLevelPreview>();
        long totalXp = 0;

        for (var level = 1; level <= maxLevel; level++)
        {
            var xpRequired = CalculateXpForLevel(curve, level);
            totalXp += xpRequired;

            previews.Add(new XpLevelPreview
            {
                Level = level,
                XpRequired = xpRequired,
                TotalXp = totalXp
            });
        }

        return previews;
    }

    public void ValidateFormula(XpCurveDefinition curve, int previewLevel)
    {
        if (curve.CurveType != XpCurveType.CustomFormula)
        {
            curve.IsFormulaValid = true;
            curve.FormulaValidationError = null;
            return;
        }

        var testValues = new Dictionary<string, double>
        {
            ["Level"] = previewLevel,
            ["BaseXP"] = curve.BaseXp
        };

        var result = formulaValidator.Validate(
            curve.Formula,
            ["Level", "BaseXP"],
            testValues);

        curve.IsFormulaValid = result.IsValid;
        curve.FormulaValidationError = result.ErrorMessage;
    }

    private long CalculateCustomXp(XpCurveDefinition curve, int level)
    {
        try
        {
            var values = new Dictionary<string, double>
            {
                ["Level"] = level,
                ["BaseXP"] = curve.BaseXp
            };

            return (long)formulaValidator.Evaluate(curve.Formula, values);
        }
        catch
        {
            return level * 100; // Fallback
        }
    }
}
