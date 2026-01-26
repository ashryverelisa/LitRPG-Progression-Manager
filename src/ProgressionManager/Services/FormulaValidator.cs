using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProgressionManager.Services;

public record FormulaValidationResult(
    bool IsValid,
    string? ErrorMessage = null,
    double? SampleResult = null);

public partial class FormulaValidator
{
    private static readonly HashSet<string> AllowedFunctions = new(StringComparer.OrdinalIgnoreCase)
    {
        "floor", "ceil", "round", "abs", "min", "max", "sqrt", "pow"
    };

    private static readonly HashSet<string> ReservedKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "Level", "BaseValue"
    };

    [GeneratedRegex(@"^[A-Za-z_][A-Za-z0-9_]*$")]
    private static partial Regex IdentifierPattern();

    [GeneratedRegex(@"[A-Za-z_][A-Za-z0-9_]*")]
    private static partial Regex VariableExtractPattern();

    [GeneratedRegex(@"^\s*$")]
    private static partial Regex EmptyPattern();

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

        var matches = VariableExtractPattern().Matches(formula);
        var unknownVariables = new List<string>();

        foreach (Match match in matches)
        {
            var identifier = match.Value;

            if (AllowedFunctions.Contains(identifier))
                continue;

            if (double.TryParse(identifier, out _))
                continue;

            if (!knownVarsSet.Contains(identifier))
            {
                unknownVariables.Add(identifier);
            }
        }

        if (unknownVariables.Count > 0)
        {
            var unknownList = string.Join(", ", unknownVariables.Distinct());
            return new FormulaValidationResult(
                false,
                $"Unknown variable(s): {unknownList}");
        }

        var parenBalance = 0;
        foreach (var c in formula)
        {
            if (c == '(') parenBalance++;
            else if (c == ')') parenBalance--;

            if (parenBalance < 0)
            {
                return new FormulaValidationResult(
                    false,
                    "Unbalanced parentheses: extra closing parenthesis");
            }
        }

        if (parenBalance > 0)
        {
            return new FormulaValidationResult(
                false,
                "Unbalanced parentheses: missing closing parenthesis");
        }

        if (HasInvalidOperatorSequence(formula))
        {
            return new FormulaValidationResult(
                false,
                "Invalid operator sequence detected");
        }

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

        var expression = PrepareExpression(formula, variables);
        return EvaluateExpression(expression);
    }

    public IEnumerable<string> ExtractVariables(string formula)
    {
        if (string.IsNullOrWhiteSpace(formula))
            return Enumerable.Empty<string>();

        var matches = VariableExtractPattern().Matches(formula);
        return matches
            .Select(m => m.Value)
            .Where(v => !AllowedFunctions.Contains(v) && !double.TryParse(v, out _))
            .Distinct();
    }

    private static bool HasInvalidOperatorSequence(string formula)
    {
        var operators = new[] { '+', '-', '*', '/', '^' };
        var lastWasOperator = false;
        var lastChar = '\0';

        foreach (var c in formula)
        {
            if (char.IsWhiteSpace(c))
                continue;

            var isOperator = operators.Contains(c);

            if (c == '-' && (lastWasOperator || lastChar == '(' || lastChar == '\0'))
            {
                lastWasOperator = false;
                lastChar = c;
                continue;
            }

            if (isOperator && lastWasOperator)
                return true;

            lastWasOperator = isOperator;
            lastChar = c;
        }

        return lastWasOperator;
    }

    private static string PrepareExpression(string formula, Dictionary<string, double> variables)
    {
        var result = formula;

        var sortedVars = variables.OrderByDescending(v => v.Key.Length);
        foreach (var (name, value) in sortedVars)
        {
            result = Regex.Replace(
                result,
                $@"\b{Regex.Escape(name)}\b",
                value.ToString(System.Globalization.CultureInfo.InvariantCulture),
                RegexOptions.IgnoreCase);
        }

        return result;
    }

    private double EvaluateExpression(string expression)
    {
        var pos = 0;
        return ParseExpression(expression.Replace(" ", ""), ref pos);
    }

    private double ParseExpression(string expr, ref int pos)
    {
        var result = ParseTerm(expr, ref pos);

        while (pos < expr.Length)
        {
            var op = expr[pos];
            if (op == '+')
            {
                pos++;
                result += ParseTerm(expr, ref pos);
            }
            else if (op == '-')
            {
                pos++;
                result -= ParseTerm(expr, ref pos);
            }
            else
            {
                break;
            }
        }

        return result;
    }

    private double ParseTerm(string expr, ref int pos)
    {
        var result = ParsePower(expr, ref pos);

        while (pos < expr.Length)
        {
            var op = expr[pos];
            if (op == '*')
            {
                pos++;
                result *= ParsePower(expr, ref pos);
            }
            else if (op == '/')
            {
                pos++;
                var divisor = ParsePower(expr, ref pos);
                if (Math.Abs(divisor) < double.Epsilon)
                    throw new DivideByZeroException("Division by zero");
                result /= divisor;
            }
            else
            {
                break;
            }
        }

        return result;
    }

    private double ParsePower(string expr, ref int pos)
    {
        var result = ParseUnary(expr, ref pos);

        if (pos < expr.Length && expr[pos] == '^')
        {
            pos++;
            var exponent = ParsePower(expr, ref pos); // Right associative
            result = Math.Pow(result, exponent);
        }

        return result;
    }

    private double ParseUnary(string expr, ref int pos)
    {
        if (pos < expr.Length && expr[pos] == '-')
        {
            pos++;
            return -ParseUnary(expr, ref pos);
        }

        if (pos < expr.Length && expr[pos] == '+')
        {
            pos++;
            return ParseUnary(expr, ref pos);
        }

        return ParsePrimary(expr, ref pos);
    }

    private double ParsePrimary(string expr, ref int pos)
    {
        var funcMatch = TryParseFunction(expr, ref pos);
        if (funcMatch.HasValue)
            return funcMatch.Value;

        if (pos < expr.Length && expr[pos] == '(')
        {
            pos++;
            var result = ParseExpression(expr, ref pos);
            if (pos < expr.Length && expr[pos] == ')')
                pos++;
            return result;
        }

        return ParseNumber(expr, ref pos);
    }

    private double? TryParseFunction(string expr, ref int pos)
    {
        foreach (var func in AllowedFunctions)
        {
            if (pos + func.Length < expr.Length &&
                expr.Substring(pos, func.Length).Equals(func, StringComparison.OrdinalIgnoreCase) &&
                expr[pos + func.Length] == '(')
            {
                pos += func.Length + 1;

                var args = new List<double>();
                args.Add(ParseExpression(expr, ref pos));

                while (pos < expr.Length && expr[pos] == ',')
                {
                    pos++;
                    args.Add(ParseExpression(expr, ref pos));
                }

                if (pos < expr.Length && expr[pos] == ')')
                    pos++;

                return EvaluateFunction(func, args);
            }
        }

        return null;
    }

    private static double EvaluateFunction(string name, List<double> args)
    {
        return name.ToLowerInvariant() switch
        {
            "floor" => Math.Floor(args[0]),
            "ceil" => Math.Ceiling(args[0]),
            "round" => Math.Round(args[0]),
            "abs" => Math.Abs(args[0]),
            "sqrt" => Math.Sqrt(args[0]),
            "min" => args.Count >= 2 ? Math.Min(args[0], args[1]) : args[0],
            "max" => args.Count >= 2 ? Math.Max(args[0], args[1]) : args[0],
            "pow" => args.Count >= 2 ? Math.Pow(args[0], args[1]) : args[0],
            _ => throw new ArgumentException($"Unknown function: {name}")
        };
    }

    private static double ParseNumber(string expr, ref int pos)
    {
        var start = pos;

        while (pos < expr.Length && (char.IsDigit(expr[pos]) || expr[pos] == '.'))
        {
            pos++;
        }

        if (start == pos)
            throw new FormatException($"Expected number at position {pos}");

        var numStr = expr.Substring(start, pos - start);
        if (!double.TryParse(numStr, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var result))
        {
            throw new FormatException($"Invalid number: {numStr}");
        }

        return result;
    }
}
