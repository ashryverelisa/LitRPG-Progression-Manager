namespace ProgressionManager.Models.WorldRules;

public record FormulaValidationResult(
    bool IsValid,
    string? ErrorMessage = null,
    double? SampleResult = null);