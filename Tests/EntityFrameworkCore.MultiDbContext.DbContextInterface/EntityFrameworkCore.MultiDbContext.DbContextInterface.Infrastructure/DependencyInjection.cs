using EntityFrameworkCore.MultiDbContext.DbContextInterface.Application.Common.Interfaces;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Common.Interfaces;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Repositories;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Infrastructure.Persistence;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Infrastructure.Repositories;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.DbContextInterface.Infrastructure
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
            services.AddDbContext<ConnStrDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("ConnStr");
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IConnStrDbContext>(provider => provider.GetRequiredService<ConnStrDbContext>());
            services.AddTransient<IAppDbEntityRepository, AppDbEntityRepository>();
            services.AddTransient<IConnstrEntityRepository, ConnstrEntityRepository>();
            services.AddTransient<IDefaultEntityRepository, DefaultEntityRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}