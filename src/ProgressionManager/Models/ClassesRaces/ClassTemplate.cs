using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Models.ClassesRaces;

/// <summary>
/// Represents a character class template with stat modifiers, equipment restrictions, and starting skills.
/// </summary>
public partial class ClassTemplate : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    /// <summary>
    /// Optional parent class ID for class inheritance/evolution.
    /// </summary>
    [ObservableProperty]
    private string? _parentClassId;

    /// <summary>
    /// Stat modifiers this class applies to base stats.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<StatModifier> _statModifiers = [];

    /// <summary>
    /// Skills that characters of this class start with at level 1.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<StartingSkill> _startingSkills = [];

    /// <summary>
    /// Equipment categories and what items are allowed for this class.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<EquipmentCategory> _allowedEquipment = [];

    /// <summary>
    /// Special bonuses this class receives at certain levels.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<LevelUpBonus> _levelUpBonuses = [];

    /// <summary>
    /// Creates a copy of this class template.
    /// </summary>
    public ClassTemplate Clone() => new()
    {
        Id = Guid.NewGuid().ToString(),
        Name = $"{Name} (Copy)",
        Description = Description,
        ParentClassId = ParentClassId,
        StatModifiers = new ObservableCollection<StatModifier>(
            StatModifiers.Select(m => m.Clone())),
        StartingSkills = new ObservableCollection<StartingSkill>(
            StartingSkills.Select(s => s.Clone())),
        AllowedEquipment = new ObservableCollection<EquipmentCategory>(
            AllowedEquipment.Select(e => e.Clone())),
        LevelUpBonuses = new ObservableCollection<LevelUpBonus>(
            LevelUpBonuses.Select(b => b.Clone()))
    };
}

