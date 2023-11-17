using System.Reflection;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Common.Eventing;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Common.Interfaces;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Repositories;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Repositories.Mapping;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Infrastructure.Configuration;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Infrastructure.Eventing;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Infrastructure.Persistence;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Infrastructure.Repositories;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Infrastructure.Repositories.Mapping;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Infrastructure
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
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IClassWithVORepository, ClassWithVORepository>();
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}