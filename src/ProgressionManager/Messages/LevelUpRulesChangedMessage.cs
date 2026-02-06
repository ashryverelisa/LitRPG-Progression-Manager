using ProgressionManager.Models.WorldRules;

namespace ProgressionManager.Messages;

/// <summary>
/// Message sent when level-up rules are modified in WorldRulesViewModel
/// </summary>
public class LevelUpRulesChangedMessage(LevelUpRules levelUpRules)
{
    public LevelUpRules LevelUpRules { get; } = levelUpRules;
}

