﻿﻿﻿﻿using Microsoft.Extensions.DependencyInjection;
using ProgressionManager.Services;
using ProgressionManager.Services.Interfaces;
using ProgressionManager.ViewModels;
using ProgressionManager.Views;

namespace ProgressionManager.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddCommonServices()
        {
            services.AddSingleton<IEmbeddedResourceService, EmbeddedResourceService>();
            services.AddScoped<IFormulaValidatorService, FormulaValidatorService>();
            services.AddScoped<IStatService, StatService>();
            services.AddScoped<IXpCurveCalculatorService, XpCurveCalculatorServiceService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<IEquipmentService, EquipmentService>();
            services.AddScoped<IClassService, ClassService>();
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

        public void AddViews()
        {
            services.AddTransient<WorldRulesView>();
            services.AddTransient<CharactersView>();
            services.AddTransient<ClassesView>();
            services.AddTransient<RacesView>();
            services.AddTransient<SkillsView>();
            services.AddTransient<TimelineView>();
            services.AddTransient<ValidationView>();
        }
    }
}