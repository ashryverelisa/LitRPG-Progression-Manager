using System.Collections.Generic;
using ProgressionManager.Models.WorldRules;

namespace ProgressionManager.Services.Interfaces;

public interface IStatService
{
    IEnumerable<StatDefinition> GetDefaultStats();
    StatDefinition CreateBaseStat(string name = "NEW_STAT");
    StatDefinition CreateDerivedStat(string name = "NEW_DERIVED");
    StatDefinition CloneStat(StatDefinition stat);
    IEnumerable<StatDefinition> FindDependentStats(StatDefinition stat, IEnumerable<StatDefinition> allStats);
    void ValidateStatFormula(StatDefinition stat, IEnumerable<StatDefinition> allStats, int previewLevel);
}
