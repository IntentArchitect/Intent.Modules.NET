using System.Reflection;
using AutoMapper;
using AzureFunctions.TestApplication.Application.Common.Behaviours;
using AzureFunctions.TestApplication.Application.Implementation;
using AzureFunctions.TestApplication.Application.Implementation.CosmosDB;
using AzureFunctions.TestApplication.Application.Implementation.Queues;
using AzureFunctions.TestApplication.Application.Implementation.Queues.Bindings;
using AzureFunctions.TestApplication.Application.Interfaces;
using AzureFunctions.TestApplication.Application.Interfaces.CosmosDB;
using AzureFunctions.TestApplication.Application.Interfaces.Queues;
using AzureFunctions.TestApplication.Application.Interfaces.Queues.Bindings;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
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
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IAzureBlobStorageService, AzureBlobStorageService>();
            services.AddTransient<IListedUnlistedServicesService, ListedUnlistedServicesService>();
            services.AddTransient<ISampleDomainsService, SampleDomainsService>();
            services.AddTransient<IChangeHandlerService, ChangeHandlerService>();
            services.AddTransient<IQueueService, QueueService>();
            services.AddTransient<IBindingService, BindingService>();
            return services;
        }
    }
}