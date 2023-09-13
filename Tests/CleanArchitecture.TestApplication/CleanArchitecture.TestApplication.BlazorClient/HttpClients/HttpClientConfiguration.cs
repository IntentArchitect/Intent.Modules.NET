using CleanArchitecture.TestApplication.BlazorClient.HttpClients.AccountService;
using CleanArchitecture.TestApplication.BlazorClient.HttpClients.HttpClients;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddHttpClient<IAggregateRootsService, AggregateRootsServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "CleanArchitectureTestApplication");
                });

            services
                .AddHttpClient<ISecuredService, SecuredServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "CleanArchitectureTestApplication");
                })
                .AddHttpMessageHandler(sp =>
                {
                    return sp.GetRequiredService<AuthorizationMessageHandler>()
                        .ConfigureHandler(
                            authorizedUrls: new[] { GetUrl(configuration, "CleanArchitectureTestApplication").AbsoluteUri });
                });

            services
                .AddHttpClient<IValidationService, ValidationServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "CleanArchitectureTestApplication");
                });

            services
                .AddHttpClient<IAccountService, AccountServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "STSApplication");
                })
                .AddHttpMessageHandler(sp =>
                {
                    return sp.GetRequiredService<AuthorizationMessageHandler>()
                    .ConfigureHandler(
                        authorizedUrls: new[] { GetUrl(configuration, "STSApplication").AbsoluteUri });
                });
        }

        private static Uri GetUrl(IConfiguration configuration, string applicationName)
        {
            var url = configuration.GetValue<Uri?>($"Urls:{applicationName}");

            return url ?? throw new Exception($"Configuration key \"Urls:{applicationName}\" is not set");
        }
    }
}