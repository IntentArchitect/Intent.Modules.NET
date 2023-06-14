using System.Reflection;
using AutoMapper;
using FluentValidation;
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
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddTransient<IInvoiceService, InvoiceService>();
            services.AddTransient<IMultiVersionService, MultiVersionService>();
            services.AddTransient<IVersionOneService, VersionOneService>();
            return services;
        }
    }
}