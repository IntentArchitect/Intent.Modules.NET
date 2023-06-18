using System.Reflection;
using AutoMapper;
using Entities.Constants.TestApplication.Application;
using Entities.Constants.TestApplication.Application.Common.Interfaces;
using Entities.Constants.TestApplication.Domain.Common.Interfaces;
using Entities.Constants.TestApplication.Domain.Repositories;
using Entities.Constants.TestApplication.Infrastructure.Persistence;
using Entities.Constants.TestApplication.Infrastructure.Repositories;
using Entities.Constants.TestApplication.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Entities.Constants.TestApplication.Infrastructure
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
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(Application.DependencyInjection).Assembly);
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<ITestClassRepository, TestClassRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}