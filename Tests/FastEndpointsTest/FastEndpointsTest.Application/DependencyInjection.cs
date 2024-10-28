using System.Reflection;
using AutoMapper;
using FastEndpointsTest.Application.Common.Behaviours;
using FastEndpointsTest.Application.Common.Validation;
using FastEndpointsTest.Application.Implementation.Headers;
using FastEndpointsTest.Application.Implementation.NamedQueryStrings;
using FastEndpointsTest.Application.Implementation.ScalarCollectionReturnType;
using FastEndpointsTest.Application.Implementation.ServiceDispatch;
using FastEndpointsTest.Application.Interfaces.Headers;
using FastEndpointsTest.Application.Interfaces.NamedQueryStrings;
using FastEndpointsTest.Application.Interfaces.ScalarCollectionReturnType;
using FastEndpointsTest.Application.Interfaces.ServiceDispatch;
using FastEndpointsTest.Domain.Services.DDD;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace FastEndpointsTest.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
                cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
                cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehaviour<,>));
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IAccountingDomainService, AccountingDomainService>();
            services.AddTransient<IDataContractDomainService, DataContractDomainService>();
            services.AddTransient<IServiceWithHeaderFieldService, ServiceWithHeaderFieldService>();
            services.AddTransient<IServiceWithNamedQueryStringService, ServiceWithNamedQueryStringService>();
            services.AddTransient<IServiceWithScalarWithCollectionReturnService, ServiceWithScalarWithCollectionReturnService>();
            services.AddTransient<IServiceDispatchService, ServiceDispatchService>();
            return services;
        }
    }
}