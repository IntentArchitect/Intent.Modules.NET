using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Eventing.Kafka
{
    internal class NugetPackages
    {
        public static INugetPackageInfo ConfluentKafka = new NugetPackageInfo("Confluent.Kafka", "2.3.0");
        public static INugetPackageInfo ConfluentSchemaRegistrySerdesJson = new NugetPackageInfo("Confluent.SchemaRegistry.Serdes.Json", "2.3.0");
        public static INugetPackageInfo MicrosoftExtensionsDependencyInjection = new NugetPackageInfo("Microsoft.Extensions.DependencyInjection", "8.0.0");
        public static INugetPackageInfo MicrosoftExtensionsHosting = new NugetPackageInfo("Microsoft.Extensions.Hosting", "8.0.0");
    }
}
