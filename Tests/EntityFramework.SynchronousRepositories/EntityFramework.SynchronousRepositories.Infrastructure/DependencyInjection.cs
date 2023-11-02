using EntityFramework.SynchronousRepositories.Application.Common.Interfaces;
using EntityFramework.SynchronousRepositories.Domain.Common.Interfaces;
using EntityFramework.SynchronousRepositories.Domain.Repositories;
using EntityFramework.SynchronousRepositories.Infrastructure.Persistence;
using EntityFramework.SynchronousRepositories.Infrastructure.Repositories;
using EntityFramework.SynchronousRepositories.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFramework.SynchronousRepositories.Infrastructure
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
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}