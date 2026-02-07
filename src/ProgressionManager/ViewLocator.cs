using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection;
using ProgressionManager.ViewModels;

namespace ProgressionManager;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var viewModelType = param.GetType();

        var viewModel = App.Services?.GetService(viewModelType) ?? param;

        var name = viewModelType.FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type == null) return new TextBlock { Text = "Not Found: " + name };

        if (App.Services?.GetService(type) is Control control)
        {
            control.DataContext = viewModel;
            return control;
        }

        // Fallback to Activator if View not registered in DI
        var view = (Control)Activator.CreateInstance(type)!;
        view.DataContext = viewModel;
        return view;
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}