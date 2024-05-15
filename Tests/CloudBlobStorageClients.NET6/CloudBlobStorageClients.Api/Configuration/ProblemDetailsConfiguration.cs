using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.ProblemDetailsConfiguration", Version = "1.0")]

namespace CloudBlobStorageClients.Api.Configuration
{
    public static class ProblemDetailsConfiguration
    {
        private static readonly JsonSerializerOptions DefaultOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            IgnoreReadOnlyFields = false,
            IgnoreReadOnlyProperties = false,
            IncludeFields = false,
            WriteIndented = false
        };
        public static IServiceCollection ConfigureProblemDetails(this IServiceCollection services)
        {
            services.AddExceptionHandler(conf => conf.ExceptionHandler = context =>
            {
                var details = new ProblemDetails
                {
                    Status = context.Response.StatusCode,
                    Type = $"https://httpstatuses.io/{context.Response.StatusCode}",
                    Title = "Internal Server Error"
                };
                details.Extensions.TryAdd("traceId", Activity.Current?.Id ?? context.TraceIdentifier);

                var env = context.RequestServices.GetService<IWebHostEnvironment>()!;
                var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (env.IsDevelopment() && exceptionFeature is not null)
                {
                    details.Detail = exceptionFeature.Error.ToString();
                }
                return context.Response.WriteAsJsonAsync(details, DefaultOptions, contentType: "application/problem+json");
            });
            return services;
        }
    }
}