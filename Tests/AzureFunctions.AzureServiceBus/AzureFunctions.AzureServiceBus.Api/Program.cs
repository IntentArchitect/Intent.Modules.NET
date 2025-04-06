using System.Configuration;
using AzureFunctions.AzureServiceBus.Api.Configuration;
using AzureFunctions.AzureServiceBus.Application;
using AzureFunctions.AzureServiceBus.Infrastructure;
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
