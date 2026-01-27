using System.Collections.Generic;
using ProgressionManager.Models.WorldRules;

namespace ProgressionManager.Services.Interfaces;

public interface IXpCurveCalculatorService
{
    long CalculateXpForLevel(XpCurveDefinition curve, int level);
    IEnumerable<XpLevelPreview> GeneratePreviews(XpCurveDefinition curve, int maxLevel = 20);
    void ValidateFormula(XpCurveDefinition curve, int previewLevel);
}
