using CleanArchitecture.TestApplication.BlazorClient.HttpClients.AggregateRootsService;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.HttpClientConfiguration", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IAggregateRootsService, AggregateRootsServiceHttpClient>(http =>
            {
                http.BaseAddress = configuration.GetValue<Uri?>("Urls:CleanArchitectureTestApplication") ?? throw new Exception("Configuration key \"Urls:CleanArchitectureTestApplication\" is not set");
            });
        }
    }
}