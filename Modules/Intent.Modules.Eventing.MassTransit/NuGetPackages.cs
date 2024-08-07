using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Eventing.MassTransit
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MassTransit(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "MassTransit",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.2.3",
                (>= 7, 0) => "8.2.1",
                (>= 6, 0) => "8.2.3",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'MassTransit'")
            });

        public static NugetPackageInfo MassTransitAbstractions(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "MassTransit.Abstractions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.2.3",
                (>= 7, 0) => "8.2.1",
                (>= 6, 0) => "8.2.3",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'MassTransit.Abstractions'")
            });

        public static NugetPackageInfo MassTransitRabbitMQ(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "MassTransit.RabbitMQ",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.2.3",
                (>= 7, 0) => "8.2.1",
                (>= 6, 0) => "8.2.3",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'MassTransit.RabbitMQ'")
            });

        public static NugetPackageInfo MassTransitAzureServiceBusCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "MassTransit.Azure.ServiceBus.Core",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.2.3",
                (>= 7, 0) => "8.2.1",
                (>= 6, 0) => "8.2.3",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'MassTransit.Azure.ServiceBus.Core'")
            });

        public static NugetPackageInfo MassTransitAmazonSQS(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "MassTransit.AmazonSQS",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.2.3",
                (>= 7, 0) => "8.2.1",
                (>= 6, 0) => "8.2.3",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'MassTransit.AmazonSQS'")
            });

        public static NugetPackageInfo MassTransitEntityFrameworkCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "MassTransit.EntityFrameworkCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.2.3",
                (>= 7, 0) => "8.2.1",
                (>= 6, 0) => "8.2.3",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'MassTransit.EntityFrameworkCore'")
            });
    }
}
