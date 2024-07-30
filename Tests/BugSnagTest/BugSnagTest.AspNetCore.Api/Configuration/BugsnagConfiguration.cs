using Bugsnag.AspNet.Core;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Bugsnag.BugsnagConfiguration", Version = "1.0")]

namespace BugSnagTest.AspNetCore.Api.Configuration;

public static class BugsnagConfiguration
{
    public static IServiceCollection ConfigureBugsnag(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddBugsnag(cfg =>
        {
            cfg.ApiKey = configuration["Bugsnag:ApiKey"];
            cfg.AppVersion = configuration["Bugsnag:AppVersion"];

            if (!string.IsNullOrWhiteSpace(configuration["Bugsnag:ReleaseStage"]))
            {
                cfg.ReleaseStage = configuration["Bugsnag:ReleaseStage"];
            }
        });
        return services;
    }

}