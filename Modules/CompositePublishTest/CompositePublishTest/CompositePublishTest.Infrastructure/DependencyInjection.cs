using CompositePublishTest.Domain.Common.Interfaces;
using CompositePublishTest.Domain.Repositories;
using CompositePublishTest.Infrastructure.Configuration;
using CompositePublishTest.Infrastructure.Persistence;
using CompositePublishTest.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CompositePublishTest.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure persistence layer
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IClientRepository, ClientRepository>();

            // Configure messaging infrastructure
            services.ConfigureCompositeMessageBus(configuration);

            return services;
        }
    }
}