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
            services.AddScoped<IFormulaValidatorService, FormulaValidatorService>();
            services.AddScoped<IStatService, StatService>();
            services.AddScoped<IXpCurveCalculatorService, XpCurveCalculatorServiceService>();
        }

        public void AddViewModels()
        {
            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<WorldRulesViewModel>();
            services.AddScoped<CharactersViewModel>();
            services.AddScoped<ClassesViewModel>();
            services.AddScoped<RacesViewModel>();
            services.AddScoped<SkillsViewModel>();
            services.AddScoped<TimelineViewModel>();
            services.AddScoped<ValidationViewModel>();
        }
    }
}