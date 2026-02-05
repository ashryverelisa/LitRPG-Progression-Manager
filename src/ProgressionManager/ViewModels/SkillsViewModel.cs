using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgressionManager.Models.Skills;
using ProgressionManager.Models.WorldRules;
using ProgressionManager.Services.Interfaces;

namespace ProgressionManager.ViewModels;

public partial class SkillsViewModel : ViewModelBase
{
    private readonly ISkillService _skillService = null!;
    private readonly IStatService _statService = null!;

    public ObservableCollection<SkillDefinition> Skills { get; } = [];
    public ObservableCollection<SkillTree> SkillTrees { get; } = [];
    public ObservableCollection<StatDefinition> Stats { get; } = [];
    public IEnumerable<SkillType> SkillTypes => Enum.GetValues<SkillType>();

    [ObservableProperty] private SkillDefinition? _selectedSkill;
    [ObservableProperty] private SkillTree? _selectedSkillTree;
    [ObservableProperty] private SkillTreeNode? _selectedSkillTreeNode;
    [ObservableProperty] private StatusEffect? _selectedStatusEffect;
    [ObservableProperty] private int _previewLevel = 10;
    [ObservableProperty] private int _previewSkillRank = 1;
    [ObservableProperty] private bool _isEditingNewSkill;
    [ObservableProperty] private int _selectedTabIndex;

    public IEnumerable<SkillTreeNode> AvailableChildNodes =>
        SelectedSkillTree == null || SelectedSkillTreeNode == null
            ? []
            : SelectedSkillTree.Nodes.Where(n => n.Id != SelectedSkillTreeNode.Id &&
                                                  !SelectedSkillTreeNode.ChildNodeIds.Contains(n.Id));

    public IEnumerable<SkillTreeNode> AvailableEvolutionNodes =>
        SelectedSkillTree == null || SelectedSkillTreeNode == null
            ? []
            : SelectedSkillTree.Nodes.Where(n => n.Id != SelectedSkillTreeNode.Id &&
                                                  !SelectedSkillTreeNode.EvolutionNodeIds.Contains(n.Id));

    public SkillsViewModel() { }

    public SkillsViewModel(ISkillService skillService, IStatService statService)
    {
        _skillService = skillService;
        _statService = statService;
        InitializeData();
    }

    private void InitializeData()
    {
        LoadStats();
        LoadSkills();
        CreateDefaultSkillTree();
        ValidateAllSkillsCommand.Execute(null);
    }

    private void LoadStats() => _statService.GetDefaultStats().ToList().ForEach(Stats.Add);

    private void LoadSkills() => _skillService.GetDefaultSkills().ToList().ForEach(Skills.Add);

    private void CreateDefaultSkillTree()
    {
        var defaultTree = _skillService.CreateSkillTree("Combat Skills");
        defaultTree.Description = "Basic combat abilities";
        SkillTrees.Add(defaultTree);
    }

    [RelayCommand]
    private void AddActiveSkill() => AddSkill(_skillService.CreateActiveSkill());

    [RelayCommand]
    private void AddPassiveSkill() => AddSkill(_skillService.CreatePassiveSkill());

    private void AddSkill(SkillDefinition skill)
    {
        Skills.Add(skill);
        SelectedSkill = skill;
        IsEditingNewSkill = true;
        ValidateSkillFormulas(skill);
    }

    [RelayCommand]
    private void DeleteSkill(SkillDefinition? skill)
    {
        if (skill == null) return;
        Skills.Remove(skill);
        if (SelectedSkill == skill)
            SelectedSkill = Skills.FirstOrDefault();
    }

    [RelayCommand]
    private void DuplicateSkill(SkillDefinition? skill)
    {
        if (skill == null) return;
        var duplicate = _skillService.CloneSkill(skill);
        Skills.Add(duplicate);
        SelectedSkill = duplicate;
        ValidateSkillFormulas(duplicate);
    }

    [RelayCommand]
    private void MoveSkillUp(SkillDefinition? skill) => MoveSkill(skill, -1);

    [RelayCommand]
    private void MoveSkillDown(SkillDefinition? skill) => MoveSkill(skill, 1);

    private void MoveSkill(SkillDefinition? skill, int direction)
    {
        if (skill == null) return;
        var index = Skills.IndexOf(skill);
        var newIndex = index + direction;
        if (newIndex >= 0 && newIndex < Skills.Count)
            Skills.Move(index, newIndex);
    }

    [RelayCommand]
    private void ValidateSkill(SkillDefinition? skill)
    {
        if (skill != null)
            ValidateSkillFormulas(skill);
    }

    [RelayCommand]
    private void ValidateAllSkills()
    {
        foreach (var skill in Skills)
            ValidateSkillFormulas(skill);
    }

    private void ValidateSkillFormulas(SkillDefinition skill) =>
        _skillService.ValidateSkillFormulas(skill, Stats, PreviewLevel, PreviewSkillRank);

    [RelayCommand]
    private void AddStatusEffect()
    {
        if (SelectedSkill == null) return;
        var effect = _skillService.CreateStatusEffect();
        SelectedSkill.StatusEffects.Add(effect);
        SelectedStatusEffect = effect;
    }

    [RelayCommand]
    private void DeleteStatusEffect(StatusEffect? effect)
    {
        if (SelectedSkill == null || effect == null) return;
        SelectedSkill.StatusEffects.Remove(effect);
        SelectedStatusEffect = SelectedSkill.StatusEffects.FirstOrDefault();
    }

    [RelayCommand]
    private void AddSkillTree()
    {
        var tree = _skillService.CreateSkillTree();
        SkillTrees.Add(tree);
        SelectedSkillTree = tree;
    }

    [RelayCommand]
    private void DeleteSkillTree(SkillTree? tree)
    {
        if (tree == null) return;
        SkillTrees.Remove(tree);
        if (SelectedSkillTree == tree)
            SelectedSkillTree = SkillTrees.FirstOrDefault();
    }

    [RelayCommand]
    private void AddNodeToTree(SkillDefinition? skill)
    {
        if (SelectedSkillTree == null) return;
        var tier = SelectedSkillTree.Nodes.Count > 0 ? SelectedSkillTree.Nodes.Max(n => n.Tier) : 0;
        var node = _skillService.CreateSkillTreeNode(skill, tier);
        SelectedSkillTree.Nodes.Add(node);
        SelectedSkillTreeNode = node;
    }

    [RelayCommand]
    private void RemoveNodeFromTree(SkillTreeNode? node)
    {
        if (SelectedSkillTree == null || node == null) return;
        SelectedSkillTree.RemoveNode(node.Id);
        if (SelectedSkillTreeNode == node)
            SelectedSkillTreeNode = SelectedSkillTree.Nodes.FirstOrDefault();
    }

    [RelayCommand]
    private void LinkChildNode(SkillTreeNode? childNode)
    {
        if (!CanLinkNode(childNode)) return;
        SelectedSkillTree!.LinkChildNode(SelectedSkillTreeNode!.Id, childNode!.Id);
    }

    [RelayCommand]
    private void UnlinkChildNode(string? childNodeId)
    {
        if (SelectedSkillTreeNode == null || string.IsNullOrEmpty(childNodeId)) return;
        SelectedSkillTreeNode.RemoveChildNode(childNodeId);
    }

    [RelayCommand]
    private void LinkEvolutionNode(SkillTreeNode? evolutionNode)
    {
        if (!CanLinkNode(evolutionNode)) return;
        SelectedSkillTree!.LinkEvolutionNode(SelectedSkillTreeNode!.Id, evolutionNode!.Id);
    }

    [RelayCommand]
    private void UnlinkEvolutionNode(string? evolutionNodeId)
    {
        if (SelectedSkillTreeNode == null || string.IsNullOrEmpty(evolutionNodeId)) return;
        SelectedSkillTreeNode.RemoveEvolutionNode(evolutionNodeId);
    }

    private bool CanLinkNode(SkillTreeNode? node) =>
        SelectedSkillTree != null &&
        SelectedSkillTreeNode != null &&
        node != null &&
        node.Id != SelectedSkillTreeNode.Id;

    partial void OnPreviewLevelChanged(int value) => ValidateAllSkillsCommand.Execute(null);

    partial void OnPreviewSkillRankChanged(int value) => ValidateAllSkillsCommand.Execute(null);

    partial void OnSelectedSkillChanged(SkillDefinition? oldValue, SkillDefinition? newValue)
    {
        if (oldValue != null)
            oldValue.PropertyChanged -= OnSkillPropertyChanged;

        if (newValue != null)
        {
            newValue.PropertyChanged += OnSkillPropertyChanged;
            PreviewSkillRank = newValue.CurrentRank;
            ValidateSkillFormulas(newValue);
        }
    }

    partial void OnSelectedSkillTreeNodeChanged(SkillTreeNode? value)
    {
        OnPropertyChanged(nameof(AvailableChildNodes));
        OnPropertyChanged(nameof(AvailableEvolutionNodes));
    }

    private void OnSkillPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not SkillDefinition skill) return;

        if (IsFormulaProperty(e.PropertyName))
            ValidateSkillFormulas(skill);
    }

    private static bool IsFormulaProperty(string? propertyName) =>
        propertyName is nameof(SkillDefinition.DamageFormula)
            or nameof(SkillDefinition.ManaCostFormula)
            or nameof(SkillDefinition.CooldownFormula)
            or nameof(SkillDefinition.PassiveEffectFormula);

    [RelayCommand]
    private void ResetToDefaults()
    {
        Skills.Clear();
        SkillTrees.Clear();
        Stats.Clear();
        InitializeData();
    }
}