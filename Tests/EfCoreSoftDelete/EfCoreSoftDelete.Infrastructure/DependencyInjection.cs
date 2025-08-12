using EfCoreSoftDelete.Domain.Common.Interfaces;
using EfCoreSoftDelete.Domain.Repositories;
using EfCoreSoftDelete.Infrastructure.Persistence;
using EfCoreSoftDelete.Infrastructure.Persistence.Interceptors;
using EfCoreSoftDelete.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EfCoreSoftDelete.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<SoftDeleteInterceptor>();
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
                options.AddInterceptors(sp.GetService<SoftDeleteInterceptor>()!);
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            return services;
        }
    }
}