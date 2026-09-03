using CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Implementations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {

            services
                .AddHttpClient<IAccountService, AccountServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "AspNetCoreIdentityAccountController");
                })
                .AddHttpMessageHandler(sp =>
                {
                    return sp.GetRequiredService<AuthorizationMessageHandler>()
                        .ConfigureHandler(
                            authorizedUrls: new[] { GetUrl(configuration, "AspNetCoreIdentityAccountController").AbsoluteUri });
                });

            services
                .AddHttpClient<IAggregateRootsService, AggregateRootsServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "CleanArchitectureComprehensive");
                });

            services
                .AddHttpClient<INamedQueryStringsService, NamedQueryStringsServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "CleanArchitectureComprehensive");
                });

            services
                .AddHttpClient<ISecuredService, SecuredServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "CleanArchitectureComprehensive");
                })
                .AddHttpMessageHandler(sp =>
                {
                    return sp.GetRequiredService<AuthorizationMessageHandler>()
                        .ConfigureHandler(
                            authorizedUrls: new[] { GetUrl(configuration, "CleanArchitectureComprehensive").AbsoluteUri });
                });

            services
                .AddHttpClient<IUniqueIndexConstraintService, UniqueIndexConstraintServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "CleanArchitectureComprehensive");
                });

            services
                .AddHttpClient<IValidationService, ValidationServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "CleanArchitectureComprehensive");
                });
        }

        public static void AddApiAuthorizationHandler(
            this IServiceCollection services,
            IConfiguration configuration,
            Func<IServiceProvider, string[], DelegatingHandler> handlerFactory)
        {
            services.AddHttpClient<IAccountService, AccountServiceHttpClient>()
                .AddHttpMessageHandler(sp =>
                {
                    return handlerFactory(sp, new[] { GetUrl(configuration, "AspNetCoreIdentityAccountController").AbsoluteUri });
                });

            services.AddHttpClient<ISecuredService, SecuredServiceHttpClient>()
                .AddHttpMessageHandler(sp =>
                {
                    return handlerFactory(sp, new[] { GetUrl(configuration, "CleanArchitectureComprehensive").AbsoluteUri });
                });
        }

        private static Uri GetUrl(IConfiguration configuration, string applicationName)
        {
            var url = configuration.GetValue<Uri?>($"Urls:{applicationName}");

            return url ?? throw new Exception($"Configuration key \"Urls:{applicationName}\" is not set");
        }
    }
}