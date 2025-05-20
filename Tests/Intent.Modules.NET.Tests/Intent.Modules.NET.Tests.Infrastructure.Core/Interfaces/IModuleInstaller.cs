using Intent.RoslynWeaver.Attributes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModularMonolith.Host.ModuleInstallerInterface", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Infrastructure.Core.Interfaces
{
    public interface IModuleInstaller
    {
        void ConfigureContainer(IServiceCollection services, IConfiguration configuration);
        void ConfigureSwagger(SwaggerGenOptions options);
        void ConfigureIntegrationEventConsumers(IRegistrationConfigurator cfg);
    }

    public static class IModuleInstallerExtensions
    {
        public static void ConfigureContainer(
            this IEnumerable<IModuleInstaller> installers,
            IServiceCollection services,
            IConfiguration configuration)
        {
            installers.ForEach(i => i.ConfigureContainer(services, configuration));
        }

        public static void ConfigureSwagger(this IEnumerable<IModuleInstaller> installers, SwaggerGenOptions options)
        {
            installers.ForEach(i => i.ConfigureSwagger(options));
        }

        public static void ConfigureIntegrationEventConsumers(
            this IEnumerable<IModuleInstaller> installers,
            IRegistrationConfigurator cfg)
        {
            installers.ForEach(i => i.ConfigureIntegrationEventConsumers(cfg));
        }

        private static void ForEach(this IEnumerable<IModuleInstaller> installers, Action<IModuleInstaller> action)
        {
            foreach (var installer in installers)
            {
                action(installer);
            }
        }
    }
}