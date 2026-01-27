using Microsoft.Extensions.DependencyInjection;
using ProgressionManager.Services;
using ProgressionManager.Services.Interfaces;
using ProgressionManager.ViewModels;

namespace ProgressionManager.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddCommonServices()
        {
            services.AddTransient<IFormulaValidatorService, FormulaValidatorService>();
            services.AddTransient<IStatService, StatService>();
            services.AddTransient<IXpCurveCalculatorService, XpCurveCalculatorServiceService>();
        }

        public void AddViewModels()
        {
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<WorldRulesViewModel>();
            services.AddTransient<CharactersViewModel>();
            services.AddTransient<ClassesViewModel>();
            services.AddTransient<RacesViewModel>();
            services.AddTransient<SkillsViewModel>();
            services.AddTransient<TimelineViewModel>();
            services.AddTransient<ValidationViewModel>();
        }
    }
}