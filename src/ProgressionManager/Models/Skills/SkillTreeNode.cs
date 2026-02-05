using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Models.Skills;

public partial class SkillTreeNode : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private SkillDefinition? _skill;

    /// <summary>
    /// The tier level in the skill tree (0 = root, higher = deeper in tree).
    /// </summary>
    [ObservableProperty]
    private int _tier;

    /// <summary>
    /// The horizontal position within the tier for layout purposes.
    /// </summary>
    [ObservableProperty]
    private int _positionInTier;

    /// <summary>
    /// IDs of child nodes that become available after unlocking this skill.
    /// These represent prerequisites - unlocking this node unlocks access to child nodes.
    /// </summary>
    public ObservableCollection<string> ChildNodeIds { get; } = [];

    /// <summary>
    /// IDs of nodes representing evolution paths for this skill.
    /// A skill can evolve into one of several variants (branching evolution).
    /// Example: Fireball → Inferno Blast OR Fireball → Fire Storm
    /// </summary>
    public ObservableCollection<string> EvolutionNodeIds { get; } = [];

    public bool HasChildren => ChildNodeIds.Count > 0;
    public bool HasEvolutions => EvolutionNodeIds.Count > 0;
    public bool IsLeafNode => !HasChildren && !HasEvolutions;

    /// <summary>
    /// Adds a child node ID if it doesn't already exist.
    /// </summary>
    /// <param name="nodeId">The ID of the child node to add.</param>
    /// <returns>True if added, false if already exists.</returns>
    public bool AddChildNode(string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId) || ChildNodeIds.Contains(nodeId))
            return false;

        ChildNodeIds.Add(nodeId);
        OnPropertyChanged(nameof(HasChildren));
        OnPropertyChanged(nameof(IsLeafNode));
        return true;
    }

    /// <summary>
    /// Removes a child node ID.
    /// </summary>
    /// <param name="nodeId">The ID of the child node to remove.</param>
    /// <returns>True if removed, false if not found.</returns>
    public bool RemoveChildNode(string nodeId)
    {
        var removed = ChildNodeIds.Remove(nodeId);
        if (!removed) return removed;
        OnPropertyChanged(nameof(HasChildren));
        OnPropertyChanged(nameof(IsLeafNode));
        return removed;
    }

    /// <summary>
    /// Adds an evolution node ID if it doesn't already exist.
    /// </summary>
    /// <param name="nodeId">The ID of the evolution node to add.</param>
    /// <returns>True if added, false if already exists.</returns>
    public bool AddEvolutionNode(string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId) || EvolutionNodeIds.Contains(nodeId))
            return false;

        EvolutionNodeIds.Add(nodeId);
        OnPropertyChanged(nameof(HasEvolutions));
        OnPropertyChanged(nameof(IsLeafNode));
        return true;
    }

    /// <summary>
    /// Removes an evolution node ID.
    /// </summary>
    /// <param name="nodeId">The ID of the evolution node to remove.</param>
    /// <returns>True if removed, false if not found.</returns>
    public bool RemoveEvolutionNode(string nodeId)
    {
        var removed = EvolutionNodeIds.Remove(nodeId);
        if (!removed) return removed;
        OnPropertyChanged(nameof(HasEvolutions));
        OnPropertyChanged(nameof(IsLeafNode));
        return removed;
    }

    /// <summary>
    /// Creates a deep clone with remapped node IDs based on an ID mapping dictionary.
    /// Useful when cloning an entire skill tree.
    /// </summary>
    /// <param name="idMapping">Dictionary mapping old node IDs to new node IDs.</param>
    public SkillTreeNode CloneWithRemappedIds(System.Collections.Generic.Dictionary<string, string> idMapping)
    {
        var clone = new SkillTreeNode
        {
            Id = idMapping.TryGetValue(Id, out var newId) ? newId : Guid.NewGuid().ToString(),
            Skill = Skill?.Clone(),
            Tier = Tier,
            PositionInTier = PositionInTier
        };

        foreach (var childId in ChildNodeIds)
        {
            var remappedId = idMapping.GetValueOrDefault(childId, childId);
            clone.ChildNodeIds.Add(remappedId);
        }

        foreach (var evolutionId in EvolutionNodeIds)
        {
            var remappedId = idMapping.GetValueOrDefault(evolutionId, evolutionId);
            clone.EvolutionNodeIds.Add(remappedId);
        }

        return clone;
    }
}
