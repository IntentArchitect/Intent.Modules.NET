using IdentityServer4CoHosted.EF;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace IdentityServer4CoHosted
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<IdentityServer4_CoHostedDbContext>(options =>
                    options.UseInMemoryDatabase("IdentityServer4_CoHosted"));
            }
            else
            {
                services.AddDbContext<IdentityServer4_CoHostedDbContext>(options =>
                {
                    options.UseSqlServer(
                        configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(IdentityServer4_CoHostedDbContext).Assembly.FullName));
                    options.UseLazyLoadingProxies();
                });
            }

            services.AddScoped<IIdentityServer4_CoHostedDbContext>(provider => provider.GetService<IdentityServer4_CoHostedDbContext>());
            return services;
        }
    }
}