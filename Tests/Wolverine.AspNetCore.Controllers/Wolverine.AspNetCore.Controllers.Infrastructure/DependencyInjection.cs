using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.AspNetCore.Controllers.Application.Common.Interfaces;
using Wolverine.AspNetCore.Controllers.Domain.Common.Interfaces;
using Wolverine.AspNetCore.Controllers.Domain.Repositories;
using Wolverine.AspNetCore.Controllers.Infrastructure.Persistence;
using Wolverine.AspNetCore.Controllers.Infrastructure.Repositories;
using Wolverine.AspNetCore.Controllers.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Infrastructure
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
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddTransient<IProductRepository, ProductRepository>();
            return services;
        }
    }
}