using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Eventing.Kafka
{
    public static class NugetPackages
    {

        public static NugetPackageInfo ConfluentKafka(IOutputTarget outputTarget) => new(
            name: "Confluent.Kafka",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "2.5.1",
            });

        public static NugetPackageInfo ConfluentSchemaRegistrySerdesJson(IOutputTarget outputTarget) => new(
            name: "Confluent.SchemaRegistry.Serdes.Json",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "2.5.1",
            });

        public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => new(
            name: "Microsoft.Extensions.DependencyInjection",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "8.0.0",
                (7, 0) => "8.0.0",
                _ => "8.0.0",
            });

        public static NugetPackageInfo MicrosoftExtensionsHosting(IOutputTarget outputTarget) => new(
            name: "Microsoft.Extensions.Hosting",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "8.0.0",
                (7, 0) => "8.0.0",
                _ => "8.0.0",
            });
    }
}
