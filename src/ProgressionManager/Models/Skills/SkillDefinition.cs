using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Models.Skills;

public partial class SkillDefinition : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private SkillType _skillType = SkillType.Active;

    [ObservableProperty]
    private int _maxRank = 5;

    [ObservableProperty]
    private int _currentRank = 1;

    // For active skills
    [ObservableProperty]
    private string? _damageFormula;

    [ObservableProperty]
    private string? _manaCostFormula;

    [ObservableProperty]
    private string? _cooldownFormula;

    // For passive skills
    [ObservableProperty]
    private string? _passiveEffectFormula;

    // Prerequisites
    [ObservableProperty]
    private int _requiredLevel = 1;

    [ObservableProperty]
    private string? _requiredClass;

    [ObservableProperty]
    private string? _prerequisiteSkillId;

    // Validation state
    [ObservableProperty]
    private bool _isDamageFormulaValid = true;

    [ObservableProperty]
    private string? _damageFormulaError;

    [ObservableProperty]
    private double? _damagePreview;

    [ObservableProperty]
    private bool _isManaCostFormulaValid = true;

    [ObservableProperty]
    private string? _manaCostFormulaError;

    [ObservableProperty]
    private double? _manaCostPreview;

    [ObservableProperty]
    private bool _isCooldownFormulaValid = true;

    [ObservableProperty]
    private string? _cooldownFormulaError;

    [ObservableProperty]
    private double? _cooldownPreview;

    [ObservableProperty]
    private bool _isPassiveFormulaValid = true;

    [ObservableProperty]
    private string? _passiveFormulaError;

    [ObservableProperty]
    private double? _passiveEffectPreview;

    public ObservableCollection<StatusEffect> StatusEffects { get; } = [];

    public bool IsActive => SkillType == SkillType.Active;

    public SkillDefinition Clone() => new()
    {
        Id = Guid.NewGuid().ToString(),
        Name = Name,
        Description = Description,
        SkillType = SkillType,
        MaxRank = MaxRank,
        CurrentRank = CurrentRank,
        DamageFormula = DamageFormula,
        ManaCostFormula = ManaCostFormula,
        CooldownFormula = CooldownFormula,
        PassiveEffectFormula = PassiveEffectFormula,
        RequiredLevel = RequiredLevel,
        RequiredClass = RequiredClass,
        PrerequisiteSkillId = PrerequisiteSkillId,
        IsDamageFormulaValid = IsDamageFormulaValid,
        DamageFormulaError = DamageFormulaError,
        IsManaCostFormulaValid = IsManaCostFormulaValid,
        ManaCostFormulaError = ManaCostFormulaError,
        IsCooldownFormulaValid = IsCooldownFormulaValid,
        CooldownFormulaError = CooldownFormulaError,
        IsPassiveFormulaValid = IsPassiveFormulaValid,
        PassiveFormulaError = PassiveFormulaError
    };
}
