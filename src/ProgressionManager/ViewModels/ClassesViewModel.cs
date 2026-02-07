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
    private readonly IClassService _classService = null!;
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

    }

    public ClassesViewModel(IClassService classService, IEquipmentService equipmentService, IStatService statService)
    {
        _classService = classService;
        _equipmentService = equipmentService;
        _statService = statService;

        LoadEquipmentCategories();
        LoadAvailableStats();
        LoadDefaultClasses();

        // Subscribe to stat changes from World Rules
        Messenger.Register(this);
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
        var classes = _classService.GetDefaultClasses(AvailableStats);
        foreach (var classTemplate in classes)
        {
            ClassTemplates.Add(classTemplate);
        }

        // Set the first class as selected by default
        SelectedClass = ClassTemplates.FirstOrDefault();

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
        var newClass = _classService.CreateClass(AvailableStats);

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

        var duplicate = _classService.CloneClass(classTemplate);
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

        // Use the first available base stat from World Rules
        var firstStat = AvailableStats.FirstOrDefault(s => !s.IsDerived);
        if (firstStat == null) return;

        SelectedClass.StatModifiers.Add(_classService.CreateStatModifier(firstStat));
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