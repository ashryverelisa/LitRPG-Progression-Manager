using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ProgressionManager.Models.ClassesRaces;
using ProgressionManager.Models.WorldRules;
using ProgressionManager.Services.Interfaces;

namespace ProgressionManager.Services;

public class ClassService(IEmbeddedResourceService resourceService) : IClassService
{
    private StatColorConfig? _colorConfig;

    private StatColorConfig ColorConfig => _colorConfig ??= LoadColorConfig();

    private StatColorConfig LoadColorConfig()
    {
        var config = resourceService.LoadResourceAsJson<StatColorConfig>("StatColors.json");
        return config ?? new StatColorConfig();
    }

    public IEnumerable<ClassTemplate> GetDefaultClasses(IEnumerable<StatDefinition> availableStats)
    {
        var classes = resourceService.LoadResourceAsJson<List<ClassTemplate>>("DefaultClasses.json");
        if (classes == null) return [];

        var statsList = availableStats.Where(s => !s.IsDerived).ToList();

        // Map loaded stat modifiers to the actual stats from WorldRules
        foreach (var classTemplate in classes)
        {
            var mappedModifiers = new ObservableCollection<StatModifier>();

            foreach (var modifier in classTemplate.StatModifiers)
            {
                // Find the matching stat in WorldRules (case-insensitive)
                var matchingStat = statsList.FirstOrDefault(s =>
                    s.Name.Equals(modifier.StatName, StringComparison.OrdinalIgnoreCase));

                if (matchingStat != null)
                {
                    mappedModifiers.Add(CreateStatModifier(matchingStat, modifier.Value));
                }
            }

            classTemplate.StatModifiers = mappedModifiers;
        }

        return classes;
    }

    public ClassTemplate CreateClass(IEnumerable<StatDefinition> availableStats, string name = "New Class")
    {
        var newClass = new ClassTemplate
        {
            Name = name,
            Description = "Enter a description for this class..."
        };

        // Initialize stat modifiers with all base stats set to 0
        foreach (var stat in availableStats.Where(s => !s.IsDerived))
        {
            newClass.StatModifiers.Add(CreateStatModifier(stat, 0));
        }

        return newClass;
    }

    public ClassTemplate CloneClass(ClassTemplate classTemplate)
    {
        return classTemplate.Clone();
    }

    public StatModifier CreateStatModifier(StatDefinition stat, int value = 0)
    {
        return new StatModifier
        {
            StatName = stat.Name,
            Value = value,
            Color = GetStatColor(stat.Name)
        };
    }

    private string GetStatColor(string statName)
    {
        var upperName = statName.ToUpperInvariant();
        if (ColorConfig.Colors.TryGetValue(upperName, out var color))
        {
            return color;
        }
        return ColorConfig.DefaultColor;
    }
}

/// <summary>
/// Configuration for stat colors loaded from JSON.
/// </summary>
public class StatColorConfig
{
    public Dictionary<string, string> Colors { get; set; } = new();
    public string DefaultColor { get; set; } = "#C0CAF5";
}

