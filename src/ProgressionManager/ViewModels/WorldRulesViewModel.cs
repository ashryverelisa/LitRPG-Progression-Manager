using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ProgressionManager.Models.WorldRules;
using StatDefinition = ProgressionManager.Models.WorldRules.StatDefinition;

namespace ProgressionManager.ViewModels;

public partial class WorldRulesViewModel : ViewModelBase
{
    public ObservableCollection<StatDefinition> Stats { get; } = new();

    [ObservableProperty] private XpCurveDefinition _xpCurve = new();

    [ObservableProperty] private XpCurveType _selectedCurveType = XpCurveType.Exponential;

    [ObservableProperty] private LevelUpRules _levelUpRules = new();

    public WorldRulesViewModel()
    {
        SeedDefaultStats();
    }

    private void SeedDefaultStats()
    {
        Stats.Add(new StatDefinition
        {
            Name = "STR",
            BaseValue = 10,
            GrowthPerLevel = 2,
            MinValue = 1,
            MaxValue = 999
        });

        Stats.Add(new StatDefinition
        {
            Name = "VIT",
            BaseValue = 8,
            GrowthPerLevel = 3,
            MinValue = 1,
            MaxValue = 999
        });

        Stats.Add(new StatDefinition
        {
            Name = "INT",
            BaseValue = 12,
            GrowthPerLevel = 2,
            MinValue = 1,
            MaxValue = 999
        });

        Stats.Add(new StatDefinition
        {
            Name = "HP",
            Formula = "VIT * 12 + Level * 5",
            MinValue = 0
        });

        Stats.Add(new StatDefinition
        {
            Name = "Mana",
            Formula = "INT * 10",
            MinValue = 0
        });
    }
}