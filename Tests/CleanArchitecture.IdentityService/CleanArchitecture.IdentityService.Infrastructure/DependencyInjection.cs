using CleanArchitecture.IdentityService.Application.Common.Interfaces;
using CleanArchitecture.IdentityService.Domain.Common.Interfaces;
using CleanArchitecture.IdentityService.Domain.Repositories;
using CleanArchitecture.IdentityService.Infrastructure.Persistence;
using CleanArchitecture.IdentityService.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Infrastructure
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
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IApplicationIdentityUserRepository, ApplicationIdentityUserRepository>();
            return services;
        }
    }
}