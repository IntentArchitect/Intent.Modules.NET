using System.Reflection;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Subscribe.CleanArchDapr.TestApplication.Application;
using Subscribe.CleanArchDapr.TestApplication.Application.Common.Eventing;
using Subscribe.CleanArchDapr.TestApplication.Application.Common.Interfaces;
using Subscribe.CleanArchDapr.TestApplication.Domain.Common.Interfaces;
using Subscribe.CleanArchDapr.TestApplication.Domain.Repositories;
using Subscribe.CleanArchDapr.TestApplication.Infrastructure.Configuration;
using Subscribe.CleanArchDapr.TestApplication.Infrastructure.Eventing;
using Subscribe.CleanArchDapr.TestApplication.Infrastructure.Persistence;
using Subscribe.CleanArchDapr.TestApplication.Infrastructure.Repositories;
using Subscribe.CleanArchDapr.TestApplication.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Infrastructure
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