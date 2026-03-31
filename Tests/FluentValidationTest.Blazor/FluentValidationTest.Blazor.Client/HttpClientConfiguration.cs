using System;
using FluentValidationTest.Blazor.Client.Implementations.ValidationScenarios.RecursiveDtos;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace FluentValidationTest.Blazor.Client
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddHttpClient<IRecursiveDtosService, RecursiveDtosServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "FluentValidationTest");
                });
        }

        private static Uri GetUrl(IConfiguration configuration, string applicationName)
        {
            var url = configuration.GetValue<Uri?>($"Urls:{applicationName}");

            return url ?? throw new Exception($"Configuration key \"Urls:{applicationName}\" is not set");
        }
    }
}