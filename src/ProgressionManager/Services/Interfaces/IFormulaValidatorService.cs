using System.Collections.Generic;
using ProgressionManager.Models.WorldRules;

namespace ProgressionManager.Services.Interfaces;

public interface IFormulaValidatorService
{
    FormulaValidationResult Validate(
        string formula,
        IEnumerable<string> knownVariables,
        Dictionary<string, double>? testValues = null);

    double Evaluate(string formula, Dictionary<string, double> variables);
    IEnumerable<string> ExtractVariables(string formula);
}