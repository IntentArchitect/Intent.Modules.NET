using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publish.CleanArchDapr.TestApplication.Application.Common.Eventing;
using Publish.CleanArchDapr.TestApplication.Application.Common.Interfaces;
using Publish.CleanArchDapr.TestApplication.Domain.Common.Interfaces;
using Publish.CleanArchDapr.TestApplication.Domain.Repositories;
using Publish.CleanArchDapr.TestApplication.Infrastructure.Eventing;
using Publish.CleanArchDapr.TestApplication.Infrastructure.Persistence;
using Publish.CleanArchDapr.TestApplication.Infrastructure.Repositories;
using Publish.CleanArchDapr.TestApplication.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Infrastructure
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
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<ApplicationDbContext>());
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IEventBus, EventBusImplementation>();
            services.AddScoped<IStateRepository, StateRepository>();
            return services;
        }
    }
}