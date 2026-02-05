using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProgressionManager.Models.Skills;
using ProgressionManager.Models.WorldRules;
using ProgressionManager.Services.Interfaces;

namespace ProgressionManager.Services;

public class SkillService(IFormulaValidatorService formulaValidator) : ISkillService
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public IEnumerable<SkillDefinition> GetDefaultSkills()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("ProgressionManager.Data.DefaultSkills.json");

        if (stream == null)
        {
            throw new InvalidOperationException("Could not find embedded resource: DefaultSkills.json");
        }

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        var skills = JsonSerializer.Deserialize<List<SkillDefinition>>(json, _jsonOptions);

        return skills ?? [];
    }

    public SkillDefinition CreateActiveSkill(string name = "NEW_SKILL")
    {
        return new SkillDefinition
        {
            Name = name,
            Description = "New active skill",
            SkillType = SkillType.Active,
            MaxRank = 10,
            CurrentRank = 1,
            DamageFormula = "INT * 2 + SkillRank * 5",
            ManaCostFormula = "20 + SkillRank * 2",
            CooldownFormula = "Max(1, 5 - SkillRank * 0.5)",
            RequiredLevel = 1
        };
    }

    public SkillDefinition CreatePassiveSkill(string name = "NEW_PASSIVE")
    {
        return new SkillDefinition
        {
            Name = name,
            Description = "New passive skill",
            SkillType = SkillType.Passive,
            MaxRank = 5,
            CurrentRank = 1,
            PassiveEffectFormula = "Level * SkillRank * 0.5",
            RequiredLevel = 1
        };
    }

    public SkillDefinition CloneSkill(SkillDefinition skill)
    {
        var clone = skill.Clone();
        clone.Name = $"{skill.Name}_Copy";
        return clone;
    }

    public StatusEffect CreateStatusEffect(string name = "NEW_EFFECT")
    {
        return new StatusEffect
        {
            Name = name,
            Description = "New status effect",
            Duration = 5,
            IsDebuff = false
        };
    }

    public void ValidateSkillFormulas(
        SkillDefinition skill,
        IEnumerable<StatDefinition> stats,
        int previewLevel,
        int skillRank)
    {
        var statsList = stats.ToList();
        var knownVariables = statsList
            .Select(s => s.Name)
            .Concat(["Level", "SkillRank", "BaseValue"])
            .ToList();

        var testValues = statsList
            .Where(s => !s.IsDerived)
            .ToDictionary(
                s => s.Name,
                s => (double)((s.BaseValue ?? 10) + (s.GrowthPerLevel ?? 0) * previewLevel))
            .Concat(new Dictionary<string, double>
            {
                ["Level"] = previewLevel,
                ["SkillRank"] = skillRank,
                ["BaseValue"] = 0
            })
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        // Define formula validations with their property setters
        var validations = new (string? Formula, Action<bool> SetValid, Action<string?> SetError, Action<double?> SetPreview)[]
        {
            (skill.DamageFormula, v => skill.IsDamageFormulaValid = v, e => skill.DamageFormulaError = e, p => skill.DamagePreview = p),
            (skill.ManaCostFormula, v => skill.IsManaCostFormulaValid = v, e => skill.ManaCostFormulaError = e, p => skill.ManaCostPreview = p),
            (skill.CooldownFormula, v => skill.IsCooldownFormulaValid = v, e => skill.CooldownFormulaError = e, p => skill.CooldownPreview = p),
            (skill.PassiveEffectFormula, v => skill.IsPassiveFormulaValid = v, e => skill.PassiveFormulaError = e, p => skill.PassiveEffectPreview = p)
        };

        foreach (var (formula, setValid, setError, setPreview) in validations)
        {
            var (isValid, error, preview) = ValidateFormula(formula, knownVariables, testValues);
            setValid(isValid);
            setError(error);
            setPreview(preview);
        }
    }

    private (bool IsValid, string? Error, double? Preview) ValidateFormula(
        string? formula,
        List<string> knownVariables,
        Dictionary<string, double> testValues)
    {
        if (string.IsNullOrWhiteSpace(formula))
            return (true, null, null);

        var result = formulaValidator.Validate(formula, knownVariables, testValues);
        return (result.IsValid, result.ErrorMessage, result.SampleResult);
    }

    public SkillTree CreateSkillTree(string name = "NEW_TREE")
    {
        return new SkillTree
        {
            Name = name,
            Description = "New skill tree"
        };
    }

    public SkillTreeNode CreateSkillTreeNode(SkillDefinition? skill = null, int tier = 0)
    {
        return new SkillTreeNode
        {
            Skill = skill,
            Tier = tier,
            PositionInTier = 0
        };
    }
}
