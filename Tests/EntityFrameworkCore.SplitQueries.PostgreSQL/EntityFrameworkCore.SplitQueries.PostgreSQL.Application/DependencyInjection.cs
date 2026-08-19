using System.Reflection;
using EntityFrameworkCore.SplitQueries.PostgreSQL.Application.Implementation.Geometry;
using EntityFrameworkCore.SplitQueries.PostgreSQL.Application.Interfaces.Geometry;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace EntityFrameworkCore.SplitQueries.PostgreSQL.Application
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IGeometryService, GeometryService>();
            return services;
        }
    }
}
