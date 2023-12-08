using Application.Identity.AccountController.UserIdentity.Domain.Common.Interfaces;
using Application.Identity.AccountController.UserIdentity.Domain.Repositories;
using Application.Identity.AccountController.UserIdentity.Infrastructure.Persistence;
using Application.Identity.AccountController.UserIdentity.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Application.Identity.AccountController.UserIdentity.Infrastructure
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
            services.AddTransient<IBespokeUserRepository, BespokeUserRepository>();
            return services;
        }
    }
}