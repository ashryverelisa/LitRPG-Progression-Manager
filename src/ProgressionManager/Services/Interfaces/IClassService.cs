using System.Collections.Generic;
using ProgressionManager.Models.ClassesRaces;
using ProgressionManager.Models.WorldRules;

namespace ProgressionManager.Services.Interfaces;

public interface IClassService
{
    IEnumerable<ClassTemplate> GetDefaultClasses(IEnumerable<StatDefinition> availableStats);
    ClassTemplate CreateClass(IEnumerable<StatDefinition> availableStats, string name = "New Class");
    ClassTemplate CloneClass(ClassTemplate classTemplate);
    StatModifier CreateStatModifier(StatDefinition stat, int value = 0);
}

