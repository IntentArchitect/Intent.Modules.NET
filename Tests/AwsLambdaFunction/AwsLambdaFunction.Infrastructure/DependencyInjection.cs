using AwsLambdaFunction.Domain.Common.Interfaces;
using AwsLambdaFunction.Domain.Repositories;
using AwsLambdaFunction.Infrastructure.Configuration;
using AwsLambdaFunction.Infrastructure.Persistence;
using AwsLambdaFunction.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AwsLambdaFunction.Infrastructure
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
            services.AddScoped<IDynAffiliateRepository, DynAffiliateDynamoDBRepository>();
            services.AddScoped<IDynClientRepository, DynClientDynamoDBRepository>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<DynamoDBUnitOfWork>();
            services.AddScoped<IDynamoDBUnitOfWork>(provider => provider.GetRequiredService<DynamoDBUnitOfWork>());
            services.AddTransient<IEfAffiliateRepository, EfAffiliateRepository>();
            services.AddTransient<IEfClientRepository, EfClientRepository>();
            services.ConfigureAws(configuration);
            return services;
        }
    }
}