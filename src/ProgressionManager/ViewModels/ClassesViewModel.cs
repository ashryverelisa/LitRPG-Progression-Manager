using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgressionManager.Models.ClassesRaces;
using ProgressionManager.Services.Interfaces;

namespace ProgressionManager.ViewModels;

public partial class ClassesViewModel : ViewModelBase
{
    private readonly IEquipmentService _equipmentService = null!;

    /// <summary>
    /// All defined class templates.
    /// </summary>
    public ObservableCollection<ClassTemplate> ClassTemplates { get; } = [];

    /// <summary>
    /// Available equipment categories for class restrictions.
    /// </summary>
    public ObservableCollection<EquipmentCategory> EquipmentCategories { get; } = [];

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

    public ClassesViewModel(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
        LoadEquipmentCategories();
        LoadDefaultClasses();
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
        // Create sample class templates based on README examples
        var warrior = new ClassTemplate
        {
            Name = "Warrior",
            Description = "A stalwart defender and powerful melee combatant. Warriors excel at physical combat and can wear heavy armor.",
            StatModifiers =
            [
                new StatModifier { StatName = "STR", Value = 5, Color = "#F7768E" },
                new StatModifier { StatName = "VIT", Value = 3, Color = "#FF9E64" },
                new StatModifier { StatName = "INT", Value = -2, Color = "#7AA2F7" }
            ],
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
            StatModifiers =
            [
                new StatModifier { StatName = "INT", Value = 6, Color = "#7AA2F7" },
                new StatModifier { StatName = "STR", Value = -3, Color = "#F7768E" },
                new StatModifier { StatName = "VIT", Value = -2, Color = "#FF9E64" }
            ],
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
            StatModifiers =
            [
                new StatModifier { StatName = "AGI", Value = 6, Color = "#9ECE6A" },
                new StatModifier { StatName = "STR", Value = 2, Color = "#F7768E" },
                new StatModifier { StatName = "VIT", Value = -2, Color = "#FF9E64" }
            ],
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

        SelectedClass.StatModifiers.Add(new StatModifier
        {
            StatName = "NEW",
            Value = 0,
            Color = "#C0CAF5"
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