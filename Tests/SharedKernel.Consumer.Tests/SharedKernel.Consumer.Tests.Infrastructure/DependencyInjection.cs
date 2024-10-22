using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Consumer.Tests.Application.Common.Interfaces;
using SharedKernel.Consumer.Tests.Domain.Common.Interfaces;
using SharedKernel.Consumer.Tests.Domain.Repositories;
using SharedKernel.Consumer.Tests.Infrastructure.Persistence;
using SharedKernel.Consumer.Tests.Infrastructure.Repositories;
using SharedKernel.Consumer.Tests.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Infrastructure
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
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            SharedKernel.Kernel.Tests.Infrastructure.DependencyInjection.AddInfrastructure(services, configuration);
            services.AddScoped<SharedKernel.Kernel.Tests.Infrastructure.Persistence.ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<SharedKernel.Kernel.Tests.Application.Common.Interfaces.IDomainEventService>(provider => provider.GetRequiredService<IDomainEventService>());
            return services;
        }
    }
}