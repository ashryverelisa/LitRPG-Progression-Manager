using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Models.Skills;

public partial class StatusEffect : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string? _effectFormula;

    [ObservableProperty]
    private double? _duration;

    [ObservableProperty]
    private bool _isDebuff;

    public StatusEffect Clone() => new()
    {
        Name = Name,
        Description = Description,
        EffectFormula = EffectFormula,
        Duration = Duration,
        IsDebuff = IsDebuff
    };
}
