using System.Reflection;
using AutoMapper;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.TestApplication.Application.Implementation;
using MongoDb.TestApplication.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MongoDb.TestApplication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddTransient<ICompoundIndexEntitiesService, CompoundIndexEntitiesService>();
            services.AddTransient<ICompoundIndexEntityMultiParentsService, CompoundIndexEntityMultiParentsService>();
            services.AddTransient<ICompoundIndexEntitySingleParentsService, CompoundIndexEntitySingleParentsService>();
            services.AddTransient<IIdTypeGuidsService, IdTypeGuidsService>();
            services.AddTransient<IIdTypeOjectIdStrsService, IdTypeOjectIdStrsService>();
            services.AddTransient<IMultikeyIndexEntitiesService, MultikeyIndexEntitiesService>();
            services.AddTransient<IMultikeyIndexEntityMultiParentsService, MultikeyIndexEntityMultiParentsService>();
            services.AddTransient<IMultikeyIndexEntitySingleParentsService, MultikeyIndexEntitySingleParentsService>();
            services.AddTransient<ISingleIndexEntitiesService, SingleIndexEntitiesService>();
            services.AddTransient<ISingleIndexEntityMultiParentsService, SingleIndexEntityMultiParentsService>();
            services.AddTransient<ISingleIndexEntitySingleParentsService, SingleIndexEntitySingleParentsService>();
            services.AddTransient<ITextIndexEntitiesService, TextIndexEntitiesService>();
            services.AddTransient<ITextIndexEntityMultiParentsService, TextIndexEntityMultiParentsService>();
            services.AddTransient<ITextIndexEntitySingleParentsService, TextIndexEntitySingleParentsService>();
            return services;
        }
    }
}