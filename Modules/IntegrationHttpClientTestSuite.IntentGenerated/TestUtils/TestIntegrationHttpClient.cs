using System;
using IntegrationHttpClientTestSuite.IntentGenerated.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationHttpClientTestSuite.IntentGenerated.TestUtils;

public static class TestIntegrationHttpClient
{
    public static IServiceProvider SetupServiceProvider()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        var services = new ServiceCollection();
        services.AddServiceProxies(config);

        var sp = services.BuildServiceProvider();

        return sp;
    }
}