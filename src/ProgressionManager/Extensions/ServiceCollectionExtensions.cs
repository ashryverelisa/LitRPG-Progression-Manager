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
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<WorldRulesViewModel>();
            services.AddSingleton<CharactersViewModel>();
            services.AddSingleton<ClassesViewModel>();
            services.AddSingleton<RacesViewModel>();
            services.AddSingleton<SkillsViewModel>();
            services.AddSingleton<TimelineViewModel>();
            services.AddSingleton<ValidationViewModel>();
        }

        public void AddViews()
        {
            services.AddSingleton<WorldRulesView>();
            services.AddSingleton<CharactersView>();
            services.AddSingleton<ClassesView>();
            services.AddSingleton<RacesView>();
            services.AddSingleton<SkillsView>();
            services.AddSingleton<TimelineView>();
            services.AddSingleton<ValidationView>();
        }
    }
}