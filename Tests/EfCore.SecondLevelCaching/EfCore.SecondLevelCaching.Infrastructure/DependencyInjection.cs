using EfCore.SecondLevelCaching.Application.Common.Interfaces;
using EfCore.SecondLevelCaching.Domain.Common.Interfaces;
using EfCore.SecondLevelCaching.Domain.Repositories;
using EfCore.SecondLevelCaching.Infrastructure.Caching;
using EfCore.SecondLevelCaching.Infrastructure.Persistence;
using EfCore.SecondLevelCaching.Infrastructure.Repositories;
using EFCoreSecondLevelCacheInterceptor;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EfCore.SecondLevelCaching.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
                options.AddInterceptors(sp.GetRequiredService<SecondLevelCacheInterceptor>());
            });
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisCache");
            });
            services.AddEFSecondLevelCache(options =>
            {
                options.UseCustomCacheProvider<DistributedCacheServiceProvider>();
            });
            services.AddSingleton<IDistributedCacheWithUnitOfWork, DistributedCacheWithUnitOfWork>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            return services;
        }
    }
}