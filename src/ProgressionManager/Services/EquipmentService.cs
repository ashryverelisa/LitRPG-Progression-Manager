using System.Collections.Generic;
using System.Text.Json;
using ProgressionManager.Models.ClassesRaces;
using ProgressionManager.Services.Interfaces;

namespace ProgressionManager.Services;

public class EquipmentService(IEmbeddedResourceService resourceService) : IEquipmentService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IEnumerable<EquipmentCategory> GetDefaultEquipmentCategories()
    {
        var categories = resourceService.LoadResourceAsJson<List<EquipmentCategory>>(
            "DefaultEquipment.json", JsonOptions);
        return categories ?? [];
    }
}

