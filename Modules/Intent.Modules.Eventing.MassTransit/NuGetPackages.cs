using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Eventing.MassTransit
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MassTransit(IOutputTarget outputTarget) => new(
            name: "MassTransit",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "8.2.3",
                (7, 0) => "8.2.1",
                _ => "8.2.3",
            });

        public static NugetPackageInfo MassTransitAbstractions(IOutputTarget outputTarget) => new(
            name: "MassTransit.Abstractions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "8.2.3",
                (7, 0) => "8.2.1",
                _ => "8.2.3",
            });

        public static NugetPackageInfo MassTransitRabbitMQ(IOutputTarget outputTarget) => new(
            name: "MassTransit.RabbitMQ",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "8.2.3",
                (7, 0) => "8.2.1",
                _ => "8.2.3",
            });

        public static NugetPackageInfo MassTransitAzureServiceBusCore(IOutputTarget outputTarget) => new(
            name: "MassTransit.Azure.ServiceBus.Core",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "8.2.3",
                (7, 0) => "8.2.1",
                _ => "8.2.3",
            });

        public static NugetPackageInfo MassTransitAmazonSQS(IOutputTarget outputTarget) => new(
            name: "MassTransit.AmazonSQS",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "8.2.3",
                (7, 0) => "8.2.1",
                _ => "8.2.3",
            });

        public static NugetPackageInfo MassTransitEntityFrameworkCore(IOutputTarget outputTarget) => new(
            name: "MassTransit.EntityFrameworkCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "8.2.3",
                (7, 0) => "8.2.1",
                _ => "8.2.3",
            });
    }
}
