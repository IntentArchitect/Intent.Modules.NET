using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Eventing.MassTransit;

public static class NuGetPackages
{
    public static readonly INugetPackageInfo MassTransit = new NugetPackageInfo("MassTransit", "8.0.6");
    public static readonly INugetPackageInfo MassTransitAbstractions = new NugetPackageInfo("MassTransit.Abstractions", "8.0.6");
    public static readonly INugetPackageInfo MassTransitRabbitMq = new NugetPackageInfo("MassTransit.RabbitMQ", "8.0.6");
    public static readonly INugetPackageInfo MassTransitAzureServiceBusCore = new NugetPackageInfo("MassTransit.Azure.ServiceBus.Core", "8.0.6");
    public static readonly INugetPackageInfo MassTransitAmazonSqs = new NugetPackageInfo("MassTransit.AmazonSQS", "8.0.6");
}