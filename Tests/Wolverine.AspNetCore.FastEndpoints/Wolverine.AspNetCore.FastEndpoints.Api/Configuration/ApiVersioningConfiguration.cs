using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FastEndpoints.AspVersioning;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.ApiVersioningConfiguration", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Api.Configuration
{
    public static class ApiVersioningConfiguration
    {
        public static IServiceCollection ConfigureApiVersioning(this IServiceCollection services)
        {
            services.AddVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
            });

            services.AddApiVersioning()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            return services;
        }
    }
}