using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Eventing.Kafka
{
    public static class NugetPackages
    {

        public static NugetPackageInfo ConfluentKafka(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Confluent.Kafka",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "2.5.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Confluent.Kafka'")
            });

        public static NugetPackageInfo ConfluentSchemaRegistrySerdesJson(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Confluent.SchemaRegistry.Serdes.Json",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "2.5.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Confluent.SchemaRegistry.Serdes.Json'")
            });

        public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.DependencyInjection",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.0",
                (>= 7, 0) => "8.0.0",
                (>= 6, 0) => "8.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.DependencyInjection'")
            });

        public static NugetPackageInfo MicrosoftExtensionsHosting(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Hosting",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.0",
                (>= 7, 0) => "8.0.0",
                (>= 6, 0) => "8.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Extensions.Hosting'")
            });
    }
}
