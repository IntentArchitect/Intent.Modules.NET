using System.Reflection;
using AutoMapper;
using Entities.PrivateSetters.EF.SqlServer.Application;
using Entities.PrivateSetters.EF.SqlServer.Domain.Common.Interfaces;
using Entities.PrivateSetters.EF.SqlServer.Domain.Repositories;
using Entities.PrivateSetters.EF.SqlServer.Infrastructure.Persistence;
using Entities.PrivateSetters.EF.SqlServer.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IAuditedRepository, AuditedRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddTransient<ISoftDeleteEntityRepository, SoftDeleteEntityRepository>();
            services.AddTransient<ITagRepository, TagRepository>();
            return services;
        }
    }
}