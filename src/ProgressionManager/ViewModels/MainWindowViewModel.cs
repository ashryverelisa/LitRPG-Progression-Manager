using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<NavigationItemViewModel> NavigationItems { get; }

    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    public MainWindowViewModel(WorldRulesViewModel worldRulesViewModel, SkillsViewModel skillsViewModel,
        ClassesViewModel classesViewModel, RacesViewModel racesViewModel,
        CharactersViewModel charactersViewModel, TimelineViewModel timelineViewModel,
        ValidationViewModel validationViewModel)
    {
        NavigationItems =
        [
            CreateNav("World Rules", "🌍", worldRulesViewModel),
            CreateNav("Skills", "⚔", skillsViewModel),
            CreateNav("Classes", "🧬", classesViewModel),
            CreateNav("Races", "🧝", racesViewModel),
            CreateNav("Characters", "👤", charactersViewModel),
            CreateNav("Timeline", "📈", timelineViewModel),
            CreateNav("Validation", "🔍", validationViewModel)
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