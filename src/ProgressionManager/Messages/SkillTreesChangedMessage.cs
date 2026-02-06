using System.Collections.Generic;
using ProgressionManager.Models.Skills;

namespace ProgressionManager.Messages;

/// <summary>
/// Message sent when skill trees are added, removed, or modified in SkillsViewModel
/// </summary>
public class SkillTreesChangedMessage(IReadOnlyList<SkillTree> skillTrees, SkillTreesChangedMessage.ChangeType type, SkillTree? changedTree = null)
{
    public IReadOnlyList<SkillTree> SkillTrees { get; } = skillTrees;
    public ChangeType Type { get; } = type;
    public SkillTree? ChangedTree { get; } = changedTree;

    public enum ChangeType
    {
        Added,
        Removed,
        Modified,
        Reset
    }
}

