using Intent.RoslynWeaver.Attributes;
using Ocelot.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ApiGateway.Ocelot.OcelotConfiguration", Version = "1.0")]

namespace OcelotTest.Api.Configuration
{
    public static class OcelotConfiguration
    {
        public static IConfigurationBuilder ConfigureOcelot(this IConfigurationBuilder configuration)
        {
            configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
            return configuration;
        }

        public static IServiceCollection ConfigureOcelot(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOcelot(configuration);
            return services;
        }
    }
}