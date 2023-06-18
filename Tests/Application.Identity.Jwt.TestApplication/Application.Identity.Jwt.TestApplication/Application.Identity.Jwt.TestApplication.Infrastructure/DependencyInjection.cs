using System.Reflection;
using Application.Identity.Jwt.TestApplication.Application;
using Application.Identity.Jwt.TestApplication.Domain.Common.Interfaces;
using Application.Identity.Jwt.TestApplication.Infrastructure.Persistence;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Application.Identity.Jwt.TestApplication.Infrastructure
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
            return services;
        }
    }
}