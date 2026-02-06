using System.Collections.Generic;
using ProgressionManager.Models.WorldRules;

namespace ProgressionManager.Messages;

/// <summary>
/// Message sent when stats are added, removed, or modified in WorldRulesViewModel
/// </summary>
public class StatsChangedMessage(IReadOnlyList<StatDefinition> stats, StatsChangedMessage.ChangeType type, StatDefinition? changedStat = null)
{
    public IReadOnlyList<StatDefinition> Stats { get; } = stats;
    public ChangeType Type { get; } = type;
    public StatDefinition? ChangedStat { get; } = changedStat;

    public enum ChangeType
    {
        Added,
        Removed,
        Modified,
        Reordered,
        Reset
    }
}
