using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public SkillsViewModel()
    {
    }

    public SkillsViewModel(ISkillService skillService, IStatService statService)
    {
        _skillService = skillService;
        _statService = statService;

        InitializeData();
    }

    private void InitializeData()
    {
        foreach (var stat in _statService.GetDefaultStats())
            Stats.Add(stat);

        foreach (var skill in _skillService.GetDefaultSkills())
            Skills.Add(skill);

        var defaultTree = _skillService.CreateSkillTree("Combat Skills");
        defaultTree.Description = "Basic combat abilities";
        SkillTrees.Add(defaultTree);

        ValidateAllSkillsCommand.Execute(null);
    }

    [RelayCommand]
    private void AddActiveSkill()
    {
        var skill = _skillService.CreateActiveSkill();
        Skills.Add(skill);
        SelectedSkill = skill;
        IsEditingNewSkill = true;
        _skillService.ValidateSkillFormulas(skill, Stats, PreviewLevel, PreviewSkillRank);
    }

    [RelayCommand]
    private void AddPassiveSkill()
    {
        var skill = _skillService.CreatePassiveSkill();
        Skills.Add(skill);
        SelectedSkill = skill;
        IsEditingNewSkill = true;
        _skillService.ValidateSkillFormulas(skill, Stats, PreviewLevel, PreviewSkillRank);
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
        _skillService.ValidateSkillFormulas(duplicate, Stats, PreviewLevel, PreviewSkillRank);
    }

    [RelayCommand]
    private void MoveSkillUp(SkillDefinition? skill)
    {
        if (skill == null) return;
        var index = Skills.IndexOf(skill);
        if (index > 0)
            Skills.Move(index, index - 1);
    }

    [RelayCommand]
    private void MoveSkillDown(SkillDefinition? skill)
    {
        if (skill == null) return;
        var index = Skills.IndexOf(skill);
        if (index >= 0 && index < Skills.Count - 1)
            Skills.Move(index, index + 1);
    }

    [RelayCommand]
    private void ValidateSkill(SkillDefinition? skill)
    {
        if (skill != null)
            _skillService.ValidateSkillFormulas(skill, Stats, PreviewLevel, PreviewSkillRank);
    }

    [RelayCommand]
    private void ValidateAllSkills()
    {
        foreach (var skill in Skills)
            _skillService.ValidateSkillFormulas(skill, Stats, PreviewLevel, PreviewSkillRank);
    }

    partial void OnPreviewLevelChanged(int value)
    {
        ValidateAllSkillsCommand.Execute(null);
    }

    partial void OnPreviewSkillRankChanged(int value)
    {
        ValidateAllSkillsCommand.Execute(null);
    }

    partial void OnSelectedSkillChanged(SkillDefinition? oldValue, SkillDefinition? newValue)
    {
        if (oldValue != null)
            oldValue.PropertyChanged -= OnSkillPropertyChanged;

        if (newValue != null)
        {
            newValue.PropertyChanged += OnSkillPropertyChanged;
            PreviewSkillRank = newValue.CurrentRank;
            _skillService.ValidateSkillFormulas(newValue, Stats, PreviewLevel, PreviewSkillRank);
        }
    }

    private void OnSkillPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is not SkillDefinition skill) return;

        if (e.PropertyName is nameof(SkillDefinition.DamageFormula)
            or nameof(SkillDefinition.ManaCostFormula)
            or nameof(SkillDefinition.CooldownFormula)
            or nameof(SkillDefinition.PassiveEffectFormula))
        {
            _skillService.ValidateSkillFormulas(skill, Stats, PreviewLevel, PreviewSkillRank);
        }
    }

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

        var node = _skillService.CreateSkillTreeNode(skill, SelectedSkillTree.Nodes.Count > 0
            ? SelectedSkillTree.Nodes.Max(n => n.Tier)
            : 0);

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
        if (SelectedSkillTree == null || SelectedSkillTreeNode == null || childNode == null) return;
        if (childNode.Id == SelectedSkillTreeNode.Id) return; // Can't link to self

        SelectedSkillTree.LinkChildNode(SelectedSkillTreeNode.Id, childNode.Id);
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
        if (SelectedSkillTree == null || SelectedSkillTreeNode == null || evolutionNode == null) return;
        if (evolutionNode.Id == SelectedSkillTreeNode.Id) return; // Can't link to self

        SelectedSkillTree.LinkEvolutionNode(SelectedSkillTreeNode.Id, evolutionNode.Id);
    }

    [RelayCommand]
    private void UnlinkEvolutionNode(string? evolutionNodeId)
    {
        if (SelectedSkillTreeNode == null || string.IsNullOrEmpty(evolutionNodeId)) return;

        SelectedSkillTreeNode.RemoveEvolutionNode(evolutionNodeId);
    }

    public IEnumerable<SkillTreeNode> GetAvailableChildNodes()
    {
        if (SelectedSkillTree == null || SelectedSkillTreeNode == null)
            return [];

        return SelectedSkillTree.Nodes
            .Where(n => n.Id != SelectedSkillTreeNode.Id && !SelectedSkillTreeNode.ChildNodeIds.Contains(n.Id));
    }

    public IEnumerable<SkillTreeNode> GetAvailableEvolutionNodes()
    {
        if (SelectedSkillTree == null || SelectedSkillTreeNode == null)
            return [];

        return SelectedSkillTree.Nodes
            .Where(n => n.Id != SelectedSkillTreeNode.Id && !SelectedSkillTreeNode.EvolutionNodeIds.Contains(n.Id));
    }

    partial void OnSelectedSkillTreeNodeChanged(SkillTreeNode? value)
    {
        OnPropertyChanged(nameof(GetAvailableChildNodes));
        OnPropertyChanged(nameof(GetAvailableEvolutionNodes));
    }

    [RelayCommand]
    private void ResetToDefaults()
    {
        Skills.Clear();
        SkillTrees.Clear();
        Stats.Clear();

        InitializeData();
    }
}