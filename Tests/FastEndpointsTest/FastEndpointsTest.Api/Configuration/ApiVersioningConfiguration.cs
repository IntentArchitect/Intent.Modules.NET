using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FastEndpoints.AspVersioning;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.ApiVersioningConfiguration", Version = "1.0")]

namespace FastEndpointsTest.Api.Configuration
{
    public static class ApiVersioningConfiguration
    {
        public static IServiceCollection ConfigureApiVersioning(this IServiceCollection services)
        {
            VersionSets.CreateApi(">>Api Version<<", v => v
                .HasApiVersion(new ApiVersion(1.0))
                .HasApiVersion(new ApiVersion(2.0)));
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