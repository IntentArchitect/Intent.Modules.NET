using System.Reflection;
using AutoMapper;
using CleanArchitecture.Dapr.Application;
using CleanArchitecture.Dapr.Application.Common.Eventing;
using CleanArchitecture.Dapr.Application.Common.Interfaces;
using CleanArchitecture.Dapr.Domain.Common.Interfaces;
using CleanArchitecture.Dapr.Domain.Repositories;
using CleanArchitecture.Dapr.Infrastructure.Configuration;
using CleanArchitecture.Dapr.Infrastructure.Eventing;
using CleanArchitecture.Dapr.Infrastructure.Persistence;
using CleanArchitecture.Dapr.Infrastructure.Repositories;
using CleanArchitecture.Dapr.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.Dapr.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IClientRepository, ClientDaprStateStoreRepository>();
            services.AddScoped<IDerivedRepository, DerivedDaprStateStoreRepository>();
            services.AddScoped<IDerivedOfTRepository, DerivedOfTDaprStateStoreRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceDaprStateStoreRepository>();
            services.AddScoped<IRegionRepository, RegionDaprStateStoreRepository>();
            services.AddScoped<ITagRepository, TagDaprStateStoreRepository>();
            services.AddScoped<DaprStateStoreUnitOfWork>();
            services.AddScoped<IDaprStateStoreUnitOfWork>(provider => provider.GetRequiredService<DaprStateStoreUnitOfWork>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IEventBus, EventBusImplementation>();
            services.AddScoped<IDaprStateStoreGenericRepository, DaprStateStoreGenericRepository>();
            services.AddHttpClients(configuration);
            return services;
        }
    }
}