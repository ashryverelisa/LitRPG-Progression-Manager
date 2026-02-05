using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Models.Skills;

/// <summary>
/// Represents a skill tree containing nodes with skills and their relationships.
/// </summary>
public partial class SkillTree : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    /// <summary>
    /// Optional class association - if set, this skill tree is only available to that class.
    /// </summary>
    [ObservableProperty]
    private string? _associatedClass;

    /// <summary>
    /// All nodes in this skill tree.
    /// </summary>
    public ObservableCollection<SkillTreeNode> Nodes { get; } = [];

    /// <summary>
    /// Finds a node by its ID.
    /// </summary>
    /// <param name="nodeId">The ID of the node to find.</param>
    /// <returns>The node if found, null otherwise.</returns>
    public SkillTreeNode? FindNode(string nodeId) => Nodes.FirstOrDefault(node => node.Id == nodeId);


    /// <summary>
    /// Removes a node from the skill tree and cleans up references.
    /// </summary>
    /// <param name="nodeId">The ID of the node to remove.</param>
    /// <returns>True if removed, false if not found.</returns>
    public bool RemoveNode(string nodeId)
    {
        var nodeToRemove = Nodes.FirstOrDefault(node => node.Id == nodeId);

        if (nodeToRemove == null)
            return false;

        // Remove references from other nodes
        foreach (var node in Nodes)
        {
            node.RemoveChildNode(nodeId);
            node.RemoveEvolutionNode(nodeId);
        }

        return Nodes.Remove(nodeToRemove);
    }

    /// <summary>
    /// Links a parent node to a child node (prerequisite relationship).
    /// </summary>
    /// <param name="parentNodeId">The ID of the parent node.</param>
    /// <param name="childNodeId">The ID of the child node.</param>
    /// <returns>True if linked, false if either node not found.</returns>
    public bool LinkChildNode(string parentNodeId, string childNodeId)
    {
        var parent = FindNode(parentNodeId);
        if (parent == null || FindNode(childNodeId) == null)
            return false;

        return parent.AddChildNode(childNodeId);
    }

    /// <summary>
    /// Links a node to an evolution node.
    /// </summary>
    /// <param name="nodeId">The ID of the source node.</param>
    /// <param name="evolutionNodeId">The ID of the evolution node.</param>
    /// <returns>True if linked, false if either node not found.</returns>
    public bool LinkEvolutionNode(string nodeId, string evolutionNodeId)
    {
        var node = FindNode(nodeId);
        if (node == null || FindNode(evolutionNodeId) == null)
            return false;

        return node.AddEvolutionNode(evolutionNodeId);
    }
}
