using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProgressionManager.ViewModels;

public partial class NavigationItemViewModel : ViewModelBase
{
    public string Title { get; }
    public string Icon { get; }
    public ObservableObject ContentViewModel { get; }

    [ObservableProperty]
    private bool _isSelected;

    public IRelayCommand SelectCommand { get; }

    public NavigationItemViewModel(
        string title,
        string icon,
        ObservableObject contentVm,
        Action<NavigationItemViewModel> onSelected)
    {
        Title = title;
        Icon = icon;
        ContentViewModel = contentVm;

        SelectCommand = new RelayCommand(() => onSelected(this));
    }
}