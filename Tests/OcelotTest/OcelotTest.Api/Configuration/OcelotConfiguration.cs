using Intent.RoslynWeaver.Attributes;
using Ocelot.Configuration.File;
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
            ConfigureDownstreamHostAndPortsPlaceholders(services, configuration);
            return services;
        }

        private static void ConfigureDownstreamHostAndPortsPlaceholders(
            IServiceCollection services,
            IConfiguration configuration)
        {
            services.PostConfigure<FileConfiguration>(fileConfiguration =>
            {
                var globalHosts = configuration.GetSection("Ocelot:Hosts").Get<GlobalHosts>();

                foreach (var route in fileConfiguration.Routes)
                {
                    ConfigureRoute(route, globalHosts);
                }
            });
        }

        private static void ConfigureRoute(FileRoute route, GlobalHosts? globalHosts)
        {
            foreach (var hostAndPort in route.DownstreamHostAndPorts)
            {
                var host = hostAndPort.Host;
                if (!host.StartsWith('{') || !host.EndsWith('}'))
                {
                    continue;
                }
                var placeHolder = host.TrimStart('{').TrimEnd('}');
                if (globalHosts?.TryGetValue(placeHolder, out var uri) != true || uri == null)
                {
                    continue;
                }
                route.DownstreamScheme = uri.Scheme;
                hostAndPort.Host = uri.Host;
                hostAndPort.Port = uri.Port;
            }
        }

        public class GlobalHosts : Dictionary<string, Uri>
        {
        }
    }
}