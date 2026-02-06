using ProgressionManager.Models.WorldRules;

namespace ProgressionManager.Messages;

/// <summary>
/// Message sent when the XP curve is modified in WorldRulesViewModel
/// </summary>
public class XpCurveChangedMessage(XpCurveDefinition xpCurve)
{
    public XpCurveDefinition XpCurve { get; } = xpCurve;
}