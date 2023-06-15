using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.TestApplication.Infrastructure.Configuration;

namespace Intent.IntegrationTest.HttpClient.StandardAspNetCore.TestUtils;

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