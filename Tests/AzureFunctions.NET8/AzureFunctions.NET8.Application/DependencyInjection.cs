using System.Reflection;
using AutoMapper;
using AzureFunctions.NET6.Application.Common.Behaviours;
using AzureFunctions.NET6.Application.Common.Validation;
using AzureFunctions.NET6.Application.Implementation;
using AzureFunctions.NET6.Application.Implementation.CosmosDB;
using AzureFunctions.NET6.Application.Implementation.Enums;
using AzureFunctions.NET6.Application.Implementation.Queues;
using AzureFunctions.NET6.Application.Implementation.Queues.Bindings;
using AzureFunctions.NET6.Application.Interfaces;
using AzureFunctions.NET6.Application.Interfaces.CosmosDB;
using AzureFunctions.NET6.Application.Interfaces.Enums;
using AzureFunctions.NET6.Application.Interfaces.Queues;
using AzureFunctions.NET6.Application.Interfaces.Queues.Bindings;
using AzureFunctions.NET8.Application.Common.Behaviours;
using AzureFunctions.NET8.Application.Common.Validation;
using AzureFunctions.NET8.Application.Implementation;
using AzureFunctions.NET8.Application.Implementation.CosmosDB;
using AzureFunctions.NET8.Application.Implementation.Enums;
using AzureFunctions.NET8.Application.Implementation.Queues;
using AzureFunctions.NET8.Application.Implementation.Queues.Bindings;
using AzureFunctions.NET8.Application.Interfaces;
using AzureFunctions.NET8.Application.Interfaces.CosmosDB;
using AzureFunctions.NET8.Application.Interfaces.Enums;
using AzureFunctions.NET8.Application.Interfaces.Queues;
using AzureFunctions.NET8.Application.Interfaces.Queues.Bindings;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AzureFunctions.NET8.Application
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
            services.AddTransient<IListedUnlistedServicesService, ListedUnlistedServicesService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<ISampleDomainsService, SampleDomainsService>();
            services.AddTransient<IChangeHandlerService, ChangeHandlerService>();
            services.AddTransient<IEnumService, EnumService>();
            services.AddTransient<IQueueService, QueueService>();
            services.AddTransient<IBindingService, BindingService>();
            return services;
        }
    }
}