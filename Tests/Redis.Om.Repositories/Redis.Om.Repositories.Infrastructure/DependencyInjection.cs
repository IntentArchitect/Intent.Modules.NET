using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redis.Om.Repositories.Application.Common.Interfaces;
using Redis.Om.Repositories.Domain.Common.Interfaces;
using Redis.Om.Repositories.Domain.Repositories;
using Redis.Om.Repositories.Infrastructure.Configuration;
using Redis.Om.Repositories.Infrastructure.Persistence;
using Redis.Om.Repositories.Infrastructure.Repositories;
using Redis.Om.Repositories.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IBaseTypeRepository, BaseTypeRedisOmRepository>();
            services.AddScoped<IClientRepository, ClientRedisOmRepository>();
            services.AddScoped<ICustomerRepository, CustomerRedisOmRepository>();
            services.AddScoped<IDerivedOfTRepository, DerivedOfTRedisOmRepository>();
            services.AddScoped<IDerivedTypeRepository, DerivedTypeRedisOmRepository>();
            services.AddScoped<IIdTestingRepository, IdTestingRedisOmRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRedisOmRepository>();
            services.AddScoped<IRegionRepository, RegionRedisOmRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<RedisOmUnitOfWork>();
            services.AddScoped<IRedisOmUnitOfWork>(provider => provider.GetRequiredService<RedisOmUnitOfWork>());
            services.ConfigureRedisOm(configuration);
            return services;
        }
    }
}