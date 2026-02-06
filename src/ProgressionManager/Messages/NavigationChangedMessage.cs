using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionManager.Messages;

/// <summary>
/// Message sent when the user navigates to a different view
/// </summary>
public class NavigationChangedMessage(ObservableObject newViewModel, string viewTitle)
{
    public ObservableObject NewViewModel { get; } = newViewModel;
    public string ViewTitle { get; } = viewTitle;
}

