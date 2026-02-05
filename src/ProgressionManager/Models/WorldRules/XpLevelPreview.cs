using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Models.WorldRules;

public partial class XpLevelPreview : ObservableObject
{
    [ObservableProperty]
    private int _level;

    [ObservableProperty]
    private long _xpRequired;

    [ObservableProperty]
    private long _totalXp;
}