using System.Reflection;
using AutoMapper;
using Finbuckle.SeparateDatabase.TestApplication.Application.Common.Validation;
using Finbuckle.SeparateDatabase.TestApplication.Application.Implementation;
using Finbuckle.SeparateDatabase.TestApplication.Application.Interfaces;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Finbuckle.SeparateDatabase.TestApplication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IUsersService, UsersService>();
            return services;
        }
    }
}