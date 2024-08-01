using System.Reflection;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Behaviours;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Eventing;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Validation;
using AdvancedMappingCrud.Repositories.Tests.Application.Implementation;
using AdvancedMappingCrud.Repositories.Tests.Application.Implementation.Customers;
using AdvancedMappingCrud.Repositories.Tests.Application.IntegrationEvents.EventHandlers.Customers;
using AdvancedMappingCrud.Repositories.Tests.Application.IntegrationEvents.EventHandlers.EnumMessage;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces.Customers;
using AdvancedMappingCrud.Repositories.Tests.Domain.Services;
using AdvancedMappingCrud.Repositories.Tests.Domain.Services.DomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Services.ExtensiveDomainServices;
using AdvancedMappingCrud.Repositories.Tests.Eventing.Messages;
using AutoMapper;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application
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
                cfg.AddOpenBehavior(typeof(EventBusPublishBehaviour<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehaviour<,>));
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<ICustomerManager, CustomerManager>();
            services.AddTransient<IPricingService, PricingService>();
            services.AddTransient<IMyDomainService, MyDomainService>();
            services.AddTransient<IExtensiveDomainService, ExtensiveDomainService>();
            services.AddTransient<IClassicDomainServiceTestsService, ClassicDomainServiceTestsService>();
            services.AddTransient<IPagingTSService, PagingTSService>();
            services.AddTransient<IProductsService, ProductsService>();
            services.AddTransient<IUploadDownloadService, UploadDownloadService>();
            services.AddTransient<IPersonService, PersonService>();
            services.AddTransient<IIntegrationEventHandler<QuoteCreatedIntegrationEvent>, QuoteCreatedIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<EnumSampleEvent>, EnumSampleHandler>();
            return services;
        }
    }
}