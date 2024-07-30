using System.Reflection;
using AutoMapper;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.TestApplication.Application.Common.Validation;
using Standard.AspNetCore.TestApplication.Application.Implementation;
using Standard.AspNetCore.TestApplication.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IClientsService, ClientsService>();
            services.AddTransient<IIntegrationService, IntegrationService>();
            services.AddTransient<IInvoicesService, InvoicesService>();
            services.AddTransient<IMultiVersionService, MultiVersionService>();
            services.AddTransient<IOpenApiIgnoreAllImplicitService, OpenApiIgnoreAllImplicitService>();
            services.AddTransient<IOpenApiIgnoreSingleService, OpenApiIgnoreSingleService>();
            services.AddTransient<IOpenApiOptOutSingleService, OpenApiOptOutSingleService>();
            services.AddTransient<IPluralsService, PluralsService>();
            services.AddTransient<IQueryStringNamesService, QueryStringNamesService>();
            services.AddTransient<IValidationTestingService, ValidationTestingService>();
            services.AddTransient<IVersionOneService, VersionOneService>();
            return services;
        }
    }
}