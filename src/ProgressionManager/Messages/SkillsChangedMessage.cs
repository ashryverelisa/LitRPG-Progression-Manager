using System.Collections.Generic;
using ProgressionManager.Models.Skills;

namespace ProgressionManager.Messages;

/// <summary>
/// Message sent when skills are added, removed, or modified in SkillsViewModel
/// </summary>
public class SkillsChangedMessage(IReadOnlyList<SkillDefinition> skills, SkillsChangedMessage.ChangeType type, SkillDefinition? changedSkill = null)
{
    public IReadOnlyList<SkillDefinition> Skills { get; } = skills;
    public ChangeType Type { get; } = type;
    public SkillDefinition? ChangedSkill { get; } = changedSkill;

    public enum ChangeType
    {
        Added,
        Removed,
        Modified,
        Reset
    }
}
