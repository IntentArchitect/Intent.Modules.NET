using System.Reflection;
using AutoMapper;
using FluentValidation;
using Integration.HttpClients.TestApplication.Application.Common.Validation;
using Integration.HttpClients.TestApplication.Application.Implementation;
using Integration.HttpClients.TestApplication.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Integration.HttpClients.TestApplication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient<IInvoiceService, InvoiceService>();
            services.AddTransient<IValidationService, ValidationService>();
            return services;
        }
    }
}