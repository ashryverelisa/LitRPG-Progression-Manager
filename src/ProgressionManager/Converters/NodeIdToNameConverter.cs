using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using ProgressionManager.Models.Skills;

namespace ProgressionManager.Converters;

/// <summary>
/// Converts a node ID to a display name by looking up the node in the skill tree.
/// </summary>
public class NodeIdToNameConverter : IMultiValueConverter
{
    public static readonly NodeIdToNameConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return values.Count > 0 ? values[0] : null;

        var nodeId = values[0] as string;
        var skillTree = values[1] as SkillTree;

        if (string.IsNullOrEmpty(nodeId) || skillTree == null)
            return nodeId;

        var node = skillTree.FindNode(nodeId);
        if (node?.Skill != null)
            return node.Skill.Name;

        // Truncate the GUID for display if node not found
        return nodeId.Length > 8 ? $"({nodeId[..8]}...)" : nodeId;
    }
}
