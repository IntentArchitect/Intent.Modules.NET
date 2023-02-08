using Integration.HttpClients.TestApplication.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Intent.IntegrationTest.HttpClient.TestUtils;

public static class TestIntegrationHttpClient
{
    public static IServiceProvider SetupServiceProvider()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        var services = new ServiceCollection();
        services.AddHttpClients(config);

        var sp = services.BuildServiceProvider();

        return sp;
    }
}