using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using AzureFunctions.NET8.Application;
using AzureFunctions.NET8.Api.Configuration;
using AzureFunctions.NET8.Infrastructure;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((ctx, services) =>
    {
        var configuration = ctx.Configuration;
        services.AddApplication(configuration);
        services.ConfigureApplicationSecurity(configuration);
        services.AddInfrastructure(configuration);
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();