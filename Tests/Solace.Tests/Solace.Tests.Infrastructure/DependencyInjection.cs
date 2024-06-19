using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solace.Tests.Application.Common.Interfaces;
using Solace.Tests.Domain.Common.Interfaces;
using Solace.Tests.Domain.Repositories;
using Solace.Tests.Infrastructure.Configuration;
using Solace.Tests.Infrastructure.Persistence;
using Solace.Tests.Infrastructure.Repositories;
using Solace.Tests.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Solace.Tests.Infrastructure
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
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IPurchaseRepository, PurchaseRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddSolaceConfiguration(configuration);
            return services;
        }
    }
}