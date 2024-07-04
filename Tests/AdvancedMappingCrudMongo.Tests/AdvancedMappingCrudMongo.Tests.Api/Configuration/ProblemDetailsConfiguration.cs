using System.Collections.Generic;
using System.Diagnostics;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.ProblemDetailsConfiguration", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Api.Configuration
{
    public static class ProblemDetailsConfiguration
    {
        public static IServiceCollection ConfigureProblemDetails(this IServiceCollection services)
        {
            services.AddProblemDetails(conf => conf.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Type = $"https://httpstatuses.io/{context.ProblemDetails.Status}";

                if (context.ProblemDetails.Status != 500) { return; }
                context.ProblemDetails.Title = "Internal Server Error";
                context.ProblemDetails.Extensions.TryAdd("traceId", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);

                var env = context.HttpContext.RequestServices.GetService<IWebHostEnvironment>()!;
                if (!env.IsDevelopment()) { return; }

                var exceptionFeature = context.HttpContext.Features.Get<IExceptionHandlerFeature>();
                if (exceptionFeature is null) { return; }
                context.ProblemDetails.Detail = exceptionFeature.Error.ToString();
            });
            return services;
        }
    }
}