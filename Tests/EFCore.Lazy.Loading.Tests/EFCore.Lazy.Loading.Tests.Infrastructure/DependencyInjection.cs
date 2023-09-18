using EFCore.Lazy.Loading.Tests.Application.Common.Interfaces;
using EFCore.Lazy.Loading.Tests.Domain.Common.Interfaces;
using EFCore.Lazy.Loading.Tests.Domain.Repositories;
using EFCore.Lazy.Loading.Tests.Infrastructure.Persistence;
using EFCore.Lazy.Loading.Tests.Infrastructure.Repositories;
using EFCore.Lazy.Loading.Tests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EFCore.Lazy.Loading.Tests.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}