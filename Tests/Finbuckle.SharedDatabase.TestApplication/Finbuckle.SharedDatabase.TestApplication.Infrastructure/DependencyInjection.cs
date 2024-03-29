using System.Reflection;
using AutoMapper;
using Finbuckle.MultiTenant;
using Finbuckle.SharedDatabase.TestApplication.Application;
using Finbuckle.SharedDatabase.TestApplication.Application.Common.Interfaces;
using Finbuckle.SharedDatabase.TestApplication.Domain.Common.Interfaces;
using Finbuckle.SharedDatabase.TestApplication.Domain.Repositories;
using Finbuckle.SharedDatabase.TestApplication.Infrastructure.Persistence;
using Finbuckle.SharedDatabase.TestApplication.Infrastructure.Repositories;
using Finbuckle.SharedDatabase.TestApplication.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Infrastructure
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
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}