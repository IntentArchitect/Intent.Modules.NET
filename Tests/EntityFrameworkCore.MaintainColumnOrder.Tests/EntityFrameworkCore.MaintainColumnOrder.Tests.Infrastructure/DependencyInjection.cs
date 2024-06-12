using EntityFrameworkCore.MaintainColumnOrder.Tests.Application.Common.Interfaces;
using EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Common.Interfaces;
using EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Repositories;
using EntityFrameworkCore.MaintainColumnOrder.Tests.Infrastructure.Persistence;
using EntityFrameworkCore.MaintainColumnOrder.Tests.Infrastructure.Repositories;
using EntityFrameworkCore.MaintainColumnOrder.Tests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFrameworkCore.MaintainColumnOrder.Tests.Infrastructure
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
            services.AddTransient<IBaseWithLastRepository, BaseWithLastRepository>();
            services.AddTransient<IBasicRepository, BasicRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IInLineVoRepository, InLineVoRepository>();
            services.AddTransient<INewClassRepository, NewClassRepository>();
            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddTransient<IVOAssociationRepository, VOAssociationRepository>();
            services.AddTransient<IWithKeyConfigRepository, WithKeyConfigRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}