using System.Reflection;
using AutoMapper;
using FluentValidation;
using Intent.Modules.NET.Tests.Module1.Application.Common.Behaviours;
using Intent.Modules.NET.Tests.Module1.Application.Common.Validation;
using Intent.Modules.NET.Tests.Module1.Application.Contracts.Interfaces;
using Intent.Modules.NET.Tests.Module1.Application.Implementation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IOrdersService, OrdersService>();
            return services;
        }
    }
}