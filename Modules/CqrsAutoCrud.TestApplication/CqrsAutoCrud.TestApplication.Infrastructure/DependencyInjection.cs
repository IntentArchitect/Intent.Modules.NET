using CqrsAutoCrud.TestApplication.Domain.Common.Interfaces;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using CqrsAutoCrud.TestApplication.Infrastructure.Persistence;
using CqrsAutoCrud.TestApplication.Infrastructure.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Infrastructure
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
            services.Configure<DbContextConfiguration>(opt => configuration.GetSection("SqlServer").Bind(opt));
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<ApplicationDbContext>());
            services.AddTransient<IA_AggregateRootRepository, A_AggregateRootRepository>();
            services.AddTransient<IA_Aggregation_ManyRepository, A_Aggregation_ManyRepository>();
            services.AddTransient<IA_Aggregation_SingleRepository, A_Aggregation_SingleRepository>();
            services.AddTransient<IAA1_Aggregation_ManyRepository, AA1_Aggregation_ManyRepository>();
            services.AddTransient<IAA1_Aggregation_SingleRepository, AA1_Aggregation_SingleRepository>();
            services.AddTransient<IAA2_Aggregation_ManyRepository, AA2_Aggregation_ManyRepository>();
            services.AddTransient<IAA2_Aggregation_SingleRepository, AA2_Aggregation_SingleRepository>();
            services.AddTransient<IAA3_Aggregation_ManyRepository, AA3_Aggregation_ManyRepository>();
            services.AddTransient<IAA3_Aggregation_SingleRepository, AA3_Aggregation_SingleRepository>();
            services.AddTransient<IAA4_Aggregation_ManyRepository, AA4_Aggregation_ManyRepository>();
            services.AddTransient<IAA4_Aggregation_SingleRepository, AA4_Aggregation_SingleRepository>();
            return services;
        }
    }
}