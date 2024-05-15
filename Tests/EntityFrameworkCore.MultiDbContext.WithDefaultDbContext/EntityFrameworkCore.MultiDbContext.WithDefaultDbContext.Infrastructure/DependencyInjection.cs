using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.Common.Interfaces;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Common.Interfaces;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Repositories;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.Persistence;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.Repositories;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AlternateConnStrDefaultDbDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("AlternateConnStrDefaultDb"),
                    b => b.MigrationsAssembly(typeof(AlternateConnStrDefaultDbDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<ICustomAlternateRepository, CustomAlternateRepository>();
            services.AddTransient<ICustomAppDefaultRepository, CustomAppDefaultRepository>();
            services.AddTransient<ICustomDefaultRepository, CustomDefaultRepository>();
            services.AddTransient<IEntityAlternateRepository, EntityAlternateRepository>();
            services.AddTransient<IEntityAppDefaultRepository, EntityAppDefaultRepository>();
            services.AddTransient<IEntityDefaultRepository, EntityDefaultRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}