using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Models.ClassesRaces;

/// <summary>
/// Represents a special bonus this class receives at a certain level.
/// </summary>
public partial class LevelUpBonus : ObservableObject
{
    /// <summary>
    /// The level at which this bonus is granted.
    /// </summary>
    [ObservableProperty]
    private int _level = 1;

    /// <summary>
    /// The type of bonus (e.g., "Skill Unlock", "Stat Bonus", "Passive Ability").
    /// </summary>
    [ObservableProperty]
    private string _bonusType = string.Empty;

    /// <summary>
    /// Description of the bonus.
    /// </summary>
    [ObservableProperty]
    private string _description = string.Empty;

    /// <summary>
    /// The value of the bonus (if applicable).
    /// </summary>
    [ObservableProperty]
    private string _value = string.Empty;

    /// <summary>
    /// Creates a copy of this level-up bonus.
    /// </summary>
    public LevelUpBonus Clone() => new()
    {
        Level = Level,
        BonusType = BonusType,
        Description = Description,
        Value = Value
    };
}

