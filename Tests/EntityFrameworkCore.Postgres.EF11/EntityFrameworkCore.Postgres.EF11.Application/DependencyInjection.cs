using System.Reflection;
using EntityFrameworkCore.Postgres.EF11.Application.Implementation;
using EntityFrameworkCore.Postgres.EF11.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.EF11.Application
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IBlogsService, BlogsService>();
            return services;
        }
    }
}