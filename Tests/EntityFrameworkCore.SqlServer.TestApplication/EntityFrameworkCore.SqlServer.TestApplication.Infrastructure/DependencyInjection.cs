using EntityFrameworkCore.SqlServer.TestApplication.Application.Common.Interfaces;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common.Interfaces;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.Associations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.ExplicitKeys;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.Indexes;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.NestedAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.TPC.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.TPC.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.TPH.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.TPT.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.TPT.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories.Associations;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories.ExplicitKeys;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories.Indexes;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories.NestedAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories.TPC.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories.TPC.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories.TPH.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories.TPT.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories.TPT.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure
{
    public static class DependencyInjection
    {
        // We're not planning on using this for this test anyway and there will be
        // clashes so just ignore generation here for now.
        [IntentManaged(Mode.Ignore)]
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<ApplicationDbContext>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}