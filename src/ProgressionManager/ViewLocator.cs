using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ProgressionManager.ViewModels;

namespace ProgressionManager;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type == null) return new TextBlock { Text = "Not Found: " + name };

        // Try to resolve from DI container first
        if (App.Services?.GetService(type) is Control control)
        {
            return control;
        }

        // Fallback to Activator if not registered in DI
        return (Control)Activator.CreateInstance(type)!;

    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}