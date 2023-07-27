using System.Reflection;
using AutoMapper;
using CleanArchitecture.TestApplication.Application;
using CleanArchitecture.TestApplication.Application.Common.Eventing;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using CleanArchitecture.TestApplication.Domain.Common.Interfaces;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.Async;
using CleanArchitecture.TestApplication.Domain.Repositories.ConventionBasedEventPublishing;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories.DDD;
using CleanArchitecture.TestApplication.Domain.Repositories.DefaultDiagram;
using CleanArchitecture.TestApplication.Domain.Repositories.Enums;
using CleanArchitecture.TestApplication.Domain.Repositories.Nullability;
using CleanArchitecture.TestApplication.Domain.Repositories.Operations;
using CleanArchitecture.TestApplication.Infrastructure.Configuration;
using CleanArchitecture.TestApplication.Infrastructure.Eventing;
using CleanArchitecture.TestApplication.Infrastructure.Persistence;
using CleanArchitecture.TestApplication.Infrastructure.Repositories;
using CleanArchitecture.TestApplication.Infrastructure.Repositories.Async;
using CleanArchitecture.TestApplication.Infrastructure.Repositories.ConventionBasedEventPublishing;
using CleanArchitecture.TestApplication.Infrastructure.Repositories.CRUD;
using CleanArchitecture.TestApplication.Infrastructure.Repositories.DDD;
using CleanArchitecture.TestApplication.Infrastructure.Repositories.DefaultDiagram;
using CleanArchitecture.TestApplication.Infrastructure.Repositories.Enums;
using CleanArchitecture.TestApplication.Infrastructure.Repositories.Nullability;
using CleanArchitecture.TestApplication.Infrastructure.Repositories.Operations;
using CleanArchitecture.TestApplication.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IAsyncOperationsClassRepository, AsyncOperationsClassRepository>();
            services.AddTransient<IIntegrationTriggeringRepository, IntegrationTriggeringRepository>();
            services.AddTransient<IAggregateRootRepository, AggregateRootRepository>();
            services.AddTransient<IAggregateRootLongRepository, AggregateRootLongRepository>();
            services.AddTransient<IAggregateSingleCRepository, AggregateSingleCRepository>();
            services.AddTransient<IAggregateTestNoIdReturnRepository, AggregateTestNoIdReturnRepository>();
            services.AddTransient<IImplicitKeyAggrRootRepository, ImplicitKeyAggrRootRepository>();
            services.AddTransient<IAccountHolderRepository, AccountHolderRepository>();
            services.AddTransient<IDataContractClassRepository, DataContractClassRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<IClassWithDefaultRepository, ClassWithDefaultRepository>();
            services.AddTransient<IClassWithEnumsRepository, ClassWithEnumsRepository>();
            services.AddTransient<INullabilityPeerRepository, NullabilityPeerRepository>();
            services.AddTransient<ITestNullablityRepository, TestNullablityRepository>();
            services.AddTransient<IOperationsClassRepository, OperationsClassRepository>();
            services.AddScoped<MassTransitEventBus>();
            services.AddTransient<IEventBus>(provider => provider.GetRequiredService<MassTransitEventBus>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddMassTransitConfiguration(configuration);
            services.AddHttpClients(configuration);
            return services;
        }
    }
}