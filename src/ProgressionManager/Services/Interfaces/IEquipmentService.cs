using System.Collections.Generic;
using ProgressionManager.Models.ClassesRaces;

namespace ProgressionManager.Services.Interfaces;

public interface IEquipmentService
{
    IEnumerable<EquipmentCategory> GetDefaultEquipmentCategories();
}

