using System.Configuration;
using AzureFunctions.NET8.Api.Configuration;
using AzureFunctions.NET8.Application;
using AzureFunctions.NET8.Infrastructure;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.Isolated.Program", Version = "1.0")]

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication((ctx, builder) =>
    {
    })
    .ConfigureServices((ctx, services) =>
    {
        var configuration = ctx.Configuration;
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.Configure<LoggerFilterOptions>(options =>
        {
            // The Application Insights SDK adds a default logging filter that instructs ILogger to capture only Warning and more severe logs. Application Insights requires an explicit override.
            // Log levels can also be configured using appsettings.json. For more information, see https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs
            const string applicationInsightsLoggerProvider = "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider";
            var toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName == applicationInsightsLoggerProvider);

            if (toRemove is not null)
            {
                options.Rules.Remove(toRemove);
            }
        });
        services.AddApplication(configuration);
        services.ConfigureApplicationSecurity(configuration);
        services.AddInfrastructure(configuration);
    })
    .Build();

host.Run();
