using BlazorServerTests.Domain.Common.Interfaces;
using BlazorServerTests.Domain.Repositories;
using BlazorServerTests.Infrastructure.Persistence;
using BlazorServerTests.Infrastructure.Repositories;
using BlazorServerTests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace BlazorServerTests.Infrastructure
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
            services.AddScoped<IScopedExecutor, ScopedExecutor>();
            services.AddScoped<IScopedMediator, ScopedMediator>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            return services;
        }
    }
}