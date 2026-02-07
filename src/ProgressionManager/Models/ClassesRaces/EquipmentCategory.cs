﻿using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Models.ClassesRaces;

public partial class EquipmentCategory : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private ObservableCollection<EquipmentItem> _items = [];

    /// <summary>
    /// Creates a copy of this equipment category.
    /// </summary>
    public EquipmentCategory Clone() => new()
    {
        Name = Name,
        Items = new ObservableCollection<EquipmentItem>(
            Items.Select(i => i.Clone()))
    };
}

public partial class EquipmentItem : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private bool _isAllowed;

    [ObservableProperty]
    private string? _description;

    /// <summary>
    /// Creates a copy of this equipment item.
    /// </summary>
    public EquipmentItem Clone() => new()
    {
        Name = Name,
        IsAllowed = IsAllowed,
        Description = Description
    };
}

