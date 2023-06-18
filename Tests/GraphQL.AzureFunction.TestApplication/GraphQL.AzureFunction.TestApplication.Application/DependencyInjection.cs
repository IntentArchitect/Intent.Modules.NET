using System.Reflection;
using AutoMapper;
using FluentValidation;
using GraphQL.AzureFunction.TestApplication.Application.Common.Validation;
using GraphQL.AzureFunction.TestApplication.Application.Implementation;
using GraphQL.AzureFunction.TestApplication.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<ICustomersService, CustomersService>();
            return services;
        }
    }
}