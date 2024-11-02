using System.Configuration;
using AzureFunctions.NET8.Api.Configuration;
using AzureFunctions.NET8.Application;
using AzureFunctions.NET8.Infrastructure;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.Isolated.Program", Version = "1.0")]

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((ctx, services) =>
    {
        var configuration = ctx.Configuration;
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddApplication(configuration);
        services.ConfigureApplicationSecurity(configuration);
        services.AddInfrastructure(configuration);
    })
    .Build();

host.Run();
