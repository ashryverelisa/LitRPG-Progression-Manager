using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ProgressionManager.Messages;
using ProgressionManager.Models.ClassesRaces;
using ProgressionManager.Models.WorldRules;
using ProgressionManager.Services.Interfaces;

namespace ProgressionManager.ViewModels;

public partial class ClassesViewModel : ViewModelBase, IRecipient<StatsChangedMessage>
{
    private readonly IEquipmentService _equipmentService = null!;
    private readonly IStatService _statService = null!;

    /// <summary>
    /// All defined class templates.
    /// </summary>
    public ObservableCollection<ClassTemplate> ClassTemplates { get; } = [];

    /// <summary>
    /// Available equipment categories for class restrictions.
    /// </summary>
    public ObservableCollection<EquipmentCategory> EquipmentCategories { get; } = [];

    /// <summary>
    /// Available stats from World Rules for stat modifiers.
    /// </summary>
    public ObservableCollection<StatDefinition> AvailableStats { get; } = [];

    /// <summary>
    /// The currently selected class template for editing.
    /// </summary>
    [ObservableProperty]
    private ClassTemplate? _selectedClass;

    /// <summary>
    /// Search filter text for the class list.
    /// </summary>
    [ObservableProperty]
    private string _searchText = string.Empty;

    /// <summary>
    /// Filtered class templates based on search text.
    /// </summary>
    public ObservableCollection<ClassTemplate> FilteredClassTemplates { get; } = [];

    // Parameterless constructor for design-time
    public ClassesViewModel()
    {
        LoadDefaultClasses();
    }

    public ClassesViewModel(IEquipmentService equipmentService, IStatService statService)
    {
        _equipmentService = equipmentService;
        _statService = statService;

        LoadEquipmentCategories();
        LoadAvailableStats();
        LoadDefaultClasses();

        // Subscribe to stat changes from World Rules
        Messenger.Register<StatsChangedMessage>(this);
    }

    /// <summary>
    /// Handle stats changed message from World Rules.
    /// </summary>
    public void Receive(StatsChangedMessage message)
    {
        // Update available stats when they change in World Rules
        AvailableStats.Clear();
        foreach (var stat in message.Stats)
        {
            AvailableStats.Add(stat);
        }
    }

    private void LoadAvailableStats()
    {
        var stats = _statService.GetDefaultStats();
        foreach (var stat in stats)
        {
            AvailableStats.Add(stat);
        }
    }

    private void LoadEquipmentCategories()
    {
        var categories = _equipmentService.GetDefaultEquipmentCategories();
        foreach (var category in categories)
        {
            EquipmentCategories.Add(category);
        }
    }

    private void LoadDefaultClasses()
    {
        // Get stat colors from available stats, or use defaults
        string GetStatColor(string statName)
        {
            // Default color mapping for common stats
            return statName.ToUpperInvariant() switch
            {
                "STR" or "STRENGTH" => "#F7768E",
                "VIT" or "VITALITY" or "CON" or "CONSTITUTION" => "#FF9E64",
                "INT" or "INTELLIGENCE" => "#7AA2F7",
                "AGI" or "AGILITY" or "DEX" or "DEXTERITY" => "#9ECE6A",
                "WIS" or "WISDOM" => "#BB9AF7",
                "LUK" or "LUCK" => "#E0AF68",
                _ => "#C0CAF5"
            };
        }

        // Get stat names from available stats if loaded, otherwise use defaults
        var statNames = AvailableStats.Count > 0
            ? AvailableStats.Where(s => !s.IsDerived).Select(s => s.Name).ToList()
            : new List<string> { "STR", "VIT", "INT", "AGI", "WIS", "LUK" };

        // Create sample class templates using available stats
        var warrior = new ClassTemplate
        {
            Name = "Warrior",
            Description = "A stalwart defender and powerful melee combatant. Warriors excel at physical combat and can wear heavy armor.",
            StatModifiers = new ObservableCollection<StatModifier>(
                CreateStatModifiersFromNames(statNames, new Dictionary<string, int>
                {
                    { "STR", 5 }, { "STRENGTH", 5 },
                    { "VIT", 3 }, { "VITALITY", 3 }, { "CON", 3 }, { "CONSTITUTION", 3 },
                    { "INT", -2 }, { "INTELLIGENCE", -2 }
                }, GetStatColor)),
            StartingSkills =
            [
                new StartingSkill { SkillName = "Power Strike", StartingRank = 1 },
                new StartingSkill { SkillName = "Block", StartingRank = 1 }
            ],
            LevelUpBonuses =
            [
                new LevelUpBonus { Level = 5, BonusType = "Skill Unlock", Description = "Unlocks Cleave ability", Value = "Cleave" },
                new LevelUpBonus { Level = 10, BonusType = "Stat Bonus", Description = "+5 STR Bonus", Value = "+5" },
                new LevelUpBonus { Level = 15, BonusType = "Passive Ability", Description = "Gains Iron Will passive", Value = "Iron Will" }
            ]
        };

        var mage = new ClassTemplate
        {
            Name = "Mage",
            Description = "A master of arcane arts who wields devastating magical spells. Mages have high intelligence but low physical stats.",
            StatModifiers = new ObservableCollection<StatModifier>(
                CreateStatModifiersFromNames(statNames, new Dictionary<string, int>
                {
                    { "INT", 6 }, { "INTELLIGENCE", 6 },
                    { "STR", -3 }, { "STRENGTH", -3 },
                    { "VIT", -2 }, { "VITALITY", -2 }, { "CON", -2 }, { "CONSTITUTION", -2 }
                }, GetStatColor)),
            StartingSkills =
            [
                new StartingSkill { SkillName = "Fireball", StartingRank = 1 },
                new StartingSkill { SkillName = "Mana Shield", StartingRank = 1 }
            ],
            LevelUpBonuses =
            [
                new LevelUpBonus { Level = 5, BonusType = "Skill Unlock", Description = "Unlocks Ice Bolt ability", Value = "Ice Bolt" },
                new LevelUpBonus { Level = 10, BonusType = "Stat Bonus", Description = "+5 INT Bonus", Value = "+5" },
                new LevelUpBonus { Level = 20, BonusType = "Skill Unlock", Description = "Unlocks Meteor Storm", Value = "Meteor Storm" }
            ]
        };

        var rogue = new ClassTemplate
        {
            Name = "Rogue",
            Description = "A swift and deadly assassin who strikes from the shadows. Rogues excel at agility and critical strikes.",
            StatModifiers = new ObservableCollection<StatModifier>(
                CreateStatModifiersFromNames(statNames, new Dictionary<string, int>
                {
                    { "AGI", 6 }, { "AGILITY", 6 }, { "DEX", 6 }, { "DEXTERITY", 6 },
                    { "STR", 2 }, { "STRENGTH", 2 },
                    { "VIT", -2 }, { "VITALITY", -2 }, { "CON", -2 }, { "CONSTITUTION", -2 }
                }, GetStatColor)),
            StartingSkills =
            [
                new StartingSkill { SkillName = "Backstab", StartingRank = 1 },
                new StartingSkill { SkillName = "Stealth", StartingRank = 1 }
            ],
            LevelUpBonuses =
            [
                new LevelUpBonus { Level = 5, BonusType = "Skill Unlock", Description = "Unlocks Poison Blade", Value = "Poison Blade" },
                new LevelUpBonus { Level = 10, BonusType = "Passive Ability", Description = "Gains Shadow Step passive", Value = "Shadow Step" }
            ]
        };

        ClassTemplates.Add(warrior);
        ClassTemplates.Add(mage);
        ClassTemplates.Add(rogue);

        // Set the first class as selected by default
        SelectedClass = warrior;

        UpdateFilteredClasses();
    }

    /// <summary>
    /// Creates stat modifiers based on available stat names and a modifier map.
    /// </summary>
    private static List<StatModifier> CreateStatModifiersFromNames(
        List<string> availableStatNames,
        Dictionary<string, int> modifierMap,
        System.Func<string, string> getColor)
    {
        var result = new List<StatModifier>();

        foreach (var statName in availableStatNames)
        {
            // Check if this stat has a modifier defined (case-insensitive)
            var upperName = statName.ToUpperInvariant();
            if (modifierMap.TryGetValue(upperName, out var value))
            {
                result.Add(new StatModifier
                {
                    StatName = statName,
                    Value = value,
                    Color = getColor(statName)
                });
            }
        }

        return result;
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = value; // Suppress unused parameter warning
        UpdateFilteredClasses();
    }

    private void UpdateFilteredClasses()
    {
        FilteredClassTemplates.Clear();

        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? ClassTemplates
            : ClassTemplates.Where(c =>
                c.Name.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase) ||
                c.Description.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase));

        foreach (var classTemplate in filtered)
        {
            FilteredClassTemplates.Add(classTemplate);
        }
    }

    [RelayCommand]
    private void AddClass()
    {
        var newClass = new ClassTemplate
        {
            Name = "New Class",
            Description = "Enter a description for this class..."
        };

        // Copy equipment categories with all items unchecked
        foreach (var category in EquipmentCategories)
        {
            newClass.AllowedEquipment.Add(category.Clone());
        }

        ClassTemplates.Add(newClass);
        SelectedClass = newClass;
        UpdateFilteredClasses();
    }

    [RelayCommand]
    private void DuplicateClass(ClassTemplate? classTemplate)
    {
        if (classTemplate == null) return;

        var duplicate = classTemplate.Clone();
        ClassTemplates.Add(duplicate);
        SelectedClass = duplicate;
        UpdateFilteredClasses();
    }

    [RelayCommand]
    private void DeleteClass(ClassTemplate? classTemplate)
    {
        if (classTemplate == null) return;

        var index = ClassTemplates.IndexOf(classTemplate);
        ClassTemplates.Remove(classTemplate);

        // Select another class if available
        SelectedClass = ClassTemplates.Count > 0 ? ClassTemplates[System.Math.Max(0, index - 1)] : null;

        UpdateFilteredClasses();
    }

    [RelayCommand]
    private void MoveClassUp()
    {
        if (SelectedClass == null) return;

        var index = ClassTemplates.IndexOf(SelectedClass);
        if (index <= 0) return;
        ClassTemplates.Move(index, index - 1);
        UpdateFilteredClasses();
    }

    [RelayCommand]
    private void MoveClassDown()
    {
        if (SelectedClass == null) return;

        var index = ClassTemplates.IndexOf(SelectedClass);
        if (index >= ClassTemplates.Count - 1) return;
        ClassTemplates.Move(index, index + 1);
        UpdateFilteredClasses();
    }

    [RelayCommand]
    private void AddStatModifier()
    {
        if (SelectedClass == null) return;

        // Use the first available stat from World Rules, or a default
        var firstStat = AvailableStats.FirstOrDefault(s => !s.IsDerived);
        var statName = firstStat?.Name ?? "NEW";

        // Get color for the stat
        var color = statName.ToUpperInvariant() switch
        {
            "STR" or "STRENGTH" => "#F7768E",
            "VIT" or "VITALITY" or "CON" or "CONSTITUTION" => "#FF9E64",
            "INT" or "INTELLIGENCE" => "#7AA2F7",
            "AGI" or "AGILITY" or "DEX" or "DEXTERITY" => "#9ECE6A",
            "WIS" or "WISDOM" => "#BB9AF7",
            "LUK" or "LUCK" => "#E0AF68",
            _ => "#C0CAF5"
        };

        SelectedClass.StatModifiers.Add(new StatModifier
        {
            StatName = statName,
            Value = 0,
            Color = color
        });
    }

    [RelayCommand]
    private void RemoveStatModifier(StatModifier? modifier)
    {
        if (SelectedClass == null || modifier == null) return;
        SelectedClass.StatModifiers.Remove(modifier);
    }

    [RelayCommand]
    private void AddStartingSkill()
    {
        if (SelectedClass == null) return;

        SelectedClass.StartingSkills.Add(new StartingSkill
        {
            SkillName = "New Skill",
            StartingRank = 1
        });
    }

    [RelayCommand]
    private void RemoveStartingSkill(StartingSkill? skill)
    {
        if (SelectedClass == null || skill == null) return;
        SelectedClass.StartingSkills.Remove(skill);
    }

    [RelayCommand]
    private void AddLevelUpBonus()
    {
        if (SelectedClass == null) return;

        SelectedClass.LevelUpBonuses.Add(new LevelUpBonus
        {
            Level = 1,
            BonusType = "Stat Bonus",
            Description = "New bonus",
            Value = ""
        });
    }

    [RelayCommand]
    private void RemoveLevelUpBonus(LevelUpBonus? bonus)
    {
        if (SelectedClass == null || bonus == null) return;
        SelectedClass.LevelUpBonuses.Remove(bonus);
    }
}