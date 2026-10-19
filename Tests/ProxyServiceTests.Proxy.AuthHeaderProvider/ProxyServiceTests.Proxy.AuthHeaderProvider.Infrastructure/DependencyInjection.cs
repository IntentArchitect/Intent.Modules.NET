using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.Common.Interfaces;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Domain.Common.Interfaces;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Infrastructure.Configuration;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Infrastructure.HttpClients;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Infrastructure.Persistence;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Infrastructure
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IAuthorizationHeaderProvider, MyCustomAuthHeaderProvider>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddHttpClients(configuration);
            return services;
        }
    }
}