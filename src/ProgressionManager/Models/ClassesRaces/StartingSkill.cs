using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Models.ClassesRaces;

/// <summary>
/// Represents a skill that a character starts with when choosing a class.
/// </summary>
public partial class StartingSkill : ObservableObject
{
    /// <summary>
    /// Reference to the skill definition ID.
    /// </summary>
    [ObservableProperty]
    private string _skillId = string.Empty;

    /// <summary>
    /// Display name of the skill.
    /// </summary>
    [ObservableProperty]
    private string _skillName = string.Empty;

    /// <summary>
    /// The starting rank for this skill.
    /// </summary>
    [ObservableProperty]
    private int _startingRank = 1;

    /// <summary>
    /// Creates a copy of this starting skill.
    /// </summary>
    public StartingSkill Clone() => new()
    {
        SkillId = SkillId,
        SkillName = SkillName,
        StartingRank = StartingRank
    };
}

