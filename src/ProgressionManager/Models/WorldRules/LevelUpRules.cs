using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Models.WorldRules;

public partial class LevelUpRules : ObservableObject
{
    [ObservableProperty]
    private int _statPointsPerLevel = 5;

    [ObservableProperty]
    private int _skillPointsPerLevel = 1;

    [ObservableProperty]
    private string _bonusRuleDescription = string.Empty;
}