using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Common.Interfaces;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Repositories;
using Standard.AspNetCore.ServiceCallHandlers.Infrastructure.Configuration;
using Standard.AspNetCore.ServiceCallHandlers.Infrastructure.Persistence;
using Standard.AspNetCore.ServiceCallHandlers.Infrastructure.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Infrastructure
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
            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}