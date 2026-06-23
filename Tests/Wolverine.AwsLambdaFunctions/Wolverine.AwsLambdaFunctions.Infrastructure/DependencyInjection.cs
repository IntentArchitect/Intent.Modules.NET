using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.AwsLambdaFunctions.Application.Common.Interfaces;
using Wolverine.AwsLambdaFunctions.Domain.Common.Interfaces;
using Wolverine.AwsLambdaFunctions.Domain.Repositories;
using Wolverine.AwsLambdaFunctions.Infrastructure.Configuration;
using Wolverine.AwsLambdaFunctions.Infrastructure.Persistence;
using Wolverine.AwsLambdaFunctions.Infrastructure.Repositories;
using Wolverine.AwsLambdaFunctions.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Wolverine.AwsLambdaFunctions.Infrastructure
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
            services.ConfigureAws(configuration);
            return services;
        }
    }
}