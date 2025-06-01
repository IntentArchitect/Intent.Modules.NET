using EntityFramework.Application.LinqExtensions.Tests.Application.Common.Interfaces;
using EntityFramework.Application.LinqExtensions.Tests.Domain.Common.Interfaces;
using EntityFramework.Application.LinqExtensions.Tests.Domain.Repositories;
using EntityFramework.Application.LinqExtensions.Tests.Infrastructure.Persistence;
using EntityFramework.Application.LinqExtensions.Tests.Infrastructure.Repositories;
using EntityFramework.Application.LinqExtensions.Tests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFramework.Application.LinqExtensions.Tests.Infrastructure
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
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}