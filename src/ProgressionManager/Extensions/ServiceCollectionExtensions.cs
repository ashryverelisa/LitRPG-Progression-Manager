using Microsoft.Extensions.DependencyInjection;
using ProgressionManager.ViewModels;

namespace ProgressionManager.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection services)
    {
        services.AddTransient<MainWindowViewModel>();
    }
}