using System.Reflection;
using AutoMapper;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.TestApplication.Application.Common.Validation;
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
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient<IIdGuidsService, IdGuidsService>();
            services.AddTransient<IIdIntsService, IdIntsService>();
            services.AddTransient<IIdLongsService, IdLongsService>();
            services.AddTransient<IIdObjectIdsService, IdObjectIdsService>();
            services.AddTransient<IValidationService, ValidationService>();
            return services;
        }
    }
}