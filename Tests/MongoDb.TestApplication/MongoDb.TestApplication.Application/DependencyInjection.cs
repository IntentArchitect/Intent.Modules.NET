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
            services.AddTransient<IIdTypeGuidsService, IdTypeGuidsService>();
            services.AddTransient<IIdTypeIntsService, IdTypeIntsService>();
            services.AddTransient<IIdTypeLongsService, IdTypeLongsService>();
            services.AddTransient<IIdTypeOjectIdStrsService, IdTypeOjectIdStrsService>();
            services.AddTransient<IValidationService, ValidationService>();
            return services;
        }
    }
}