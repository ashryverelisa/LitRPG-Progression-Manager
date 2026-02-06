namespace ProgressionManager.Messages;

/// <summary>
/// Message requesting all ViewModels to revalidate their formulas
/// </summary>
public class ValidationRequestMessage(int previewLevel, ValidationRequestMessage.ValidationType type = ValidationRequestMessage.ValidationType.All)
{
    public int PreviewLevel { get; } = previewLevel;
    public ValidationType Type { get; } = type;

    public enum ValidationType
    {
        All,
        StatsOnly,
        SkillsOnly
    }
}
