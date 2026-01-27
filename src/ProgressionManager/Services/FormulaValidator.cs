using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NCalc;
using ProgressionManager.Models.WorldRules;

namespace ProgressionManager.Services;

public partial class FormulaValidator
{
    private static readonly HashSet<string> ReservedKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "Level", "BaseValue"
    };

    [GeneratedRegex(@"[A-Za-z_][A-Za-z0-9_]*")]
    private static partial Regex VariableExtractPattern();

    // NCalc built-in functions that should not be treated as variables
    private static readonly HashSet<string> NCalcBuiltInFunctions = new(StringComparer.OrdinalIgnoreCase)
    {
        // Math functions
        "Abs", "Acos", "Asin", "Atan", "Ceiling", "Cos", "Exp", "Floor", "IEEERemainder",
        "Log", "Log10", "Max", "Min", "Pow", "Round", "Sign", "Sin", "Sqrt", "Tan", "Truncate",
        // Additional functions
        "if", "in"
    };

    public FormulaValidationResult Validate(
        string formula,
        IEnumerable<string> knownVariables,
        Dictionary<string, double>? testValues = null)
    {
        if (string.IsNullOrWhiteSpace(formula))
        {
            return new FormulaValidationResult(true, null, null);
        }

        var knownVarsSet = new HashSet<string>(knownVariables, StringComparer.OrdinalIgnoreCase);

        foreach (var keyword in ReservedKeywords)
        {
            knownVarsSet.Add(keyword);
        }

        // Check for unknown variables first
        var extractedVars = ExtractVariables(formula);
        var unknownVariables = extractedVars
            .Where(v => !knownVarsSet.Contains(v))
            .ToList();

        if (unknownVariables.Count > 0)
        {
            var unknownList = string.Join(", ", unknownVariables.Distinct());
            return new FormulaValidationResult(
                false,
                $"Unknown variable(s): {unknownList}");
        }

        // Use NCalc to validate the expression syntax
        try
        {
            var expression = new Expression(formula, ExpressionOptions.DecimalAsDefault);

            if (expression.HasErrors())
            {
                return new FormulaValidationResult(
                    false,
                    $"Syntax error: {expression.Error}");
            }
        }
        catch (Exception ex)
        {
            return new FormulaValidationResult(
                false,
                $"Syntax error: {ex.Message}");
        }

        // If test values are provided, evaluate the expression
        if (testValues != null)
        {
            try
            {
                var result = Evaluate(formula, testValues);
                return new FormulaValidationResult(true, null, result);
            }
            catch (Exception ex)
            {
                return new FormulaValidationResult(
                    false,
                    $"Evaluation error: {ex.Message}");
            }
        }

        return new FormulaValidationResult(true);
    }

    public double Evaluate(string formula, Dictionary<string, double> variables)
    {
        if (string.IsNullOrWhiteSpace(formula))
            return 0;

        var expression = new Expression(formula, ExpressionOptions.DecimalAsDefault);

        foreach (var (name, value) in variables)
        {
            expression.Parameters[name] = value;
        }

        var result = expression.Evaluate();

        return result switch
        {
            double d => d,
            decimal dec => (double)dec,
            int i => i,
            long l => l,
            float f => f,
            _ => Convert.ToDouble(result)
        };
    }

    public IEnumerable<string> ExtractVariables(string formula)
    {
        if (string.IsNullOrWhiteSpace(formula))
            return Enumerable.Empty<string>();

        var matches = VariableExtractPattern().Matches(formula);
        return matches
            .Select(m => m.Value)
            .Where(v => !NCalcBuiltInFunctions.Contains(v) && !double.TryParse(v, out _) && !IsKeyword(v))
            .Distinct();
    }

    private static bool IsKeyword(string identifier)
    {
        // NCalc keywords like "true", "false", "and", "or", "not"
        return identifier.Equals("true", StringComparison.OrdinalIgnoreCase) ||
               identifier.Equals("false", StringComparison.OrdinalIgnoreCase) ||
               identifier.Equals("and", StringComparison.OrdinalIgnoreCase) ||
               identifier.Equals("or", StringComparison.OrdinalIgnoreCase) ||
               identifier.Equals("not", StringComparison.OrdinalIgnoreCase);
    }
}
