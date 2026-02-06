using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace ProgressionManager.ViewModels;

public class ViewModelBase : ObservableObject, IDisposable
{
    protected IMessenger Messenger { get; } = WeakReferenceMessenger.Default;

    private bool _disposed;

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Unregister all messages for this recipient
            Messenger.UnregisterAll(this);
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}