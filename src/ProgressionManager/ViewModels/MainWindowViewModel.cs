using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<NavigationItemViewModel> NavigationItems { get; }

    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    public MainWindowViewModel(WorldRulesViewModel worldRulesViewModel)
    {
        NavigationItems =
        [
            CreateNav("World Rules", "🌍", worldRulesViewModel),
            CreateNav("Skills", "⚔", new SkillsViewModel()),
            CreateNav("Classes", "🧬", new ClassesViewModel()),
            CreateNav("Races", "🧝", new RacesViewModel()),
            CreateNav("Characters", "👤", new CharactersViewModel()),
            CreateNav("Timeline", "📈", new TimelineViewModel()),
            CreateNav("Validation", "🔍", new ValidationViewModel())
        ];

        Select(NavigationItems[0]);
    }

    private NavigationItemViewModel CreateNav(
        string title,
        string icon,
        ObservableObject vm)
    {
        return new NavigationItemViewModel(
            title,
            icon,
            vm,
            Select);
    }

    private void Select(NavigationItemViewModel item)
    {
        foreach (var nav in NavigationItems)
            nav.IsSelected = false;

        item.IsSelected = true;
        CurrentViewModel = item.ContentViewModel;
    }
}