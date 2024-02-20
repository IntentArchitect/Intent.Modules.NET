using System;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.MessageBrokers;

internal class AzureServiceBusMessageBroker : MessageBrokerBase
{
    public AzureServiceBusMessageBroker(ICSharpFileBuilderTemplate template) : base(template)
    {
    }

    public override bool HasMessageBrokerStereotype(Subscription subscription)
    {
        return subscription.AzureConsumerSettings is not null;
    }

    public override string GetMessageBrokerBusFactoryConfiguratorName()
    {
        return "IServiceBusBusFactoryConfigurator";
    }

    public override string GetMessageBrokerReceiveEndpointConfiguratorName()
    {
        return "IServiceBusReceiveEndpointConfigurator";
    }

    public override IEnumerable<CSharpStatement> AddBespokeConsumerConfigurationStatements(string configVarName, Subscription subscription)
    {
        var busConsumerSettings = subscription.AzureConsumerSettings!;
        
        if (busConsumerSettings.PrefetchCount().HasValue)
        {
            yield return $@"{configVarName}.PrefetchCount = {busConsumerSettings.PrefetchCount()};";
        }

        yield return $@"{configVarName}.RequiresSession = {busConsumerSettings.RequiresSession().ToString().ToLower()};";
        if (!string.IsNullOrWhiteSpace(busConsumerSettings.DefaultMessageTimeToLive()))
        {
            ValidateTimeSpanString(busConsumerSettings.DefaultMessageTimeToLive(), nameof(busConsumerSettings.DefaultMessageTimeToLive), out var ts);
            yield return $@"{configVarName}.DefaultMessageTimeToLive = TimeSpan.Parse(""{ts}"");";
        }

        if (!string.IsNullOrWhiteSpace(busConsumerSettings.LockDuration()))
        {
            ValidateTimeSpanString(busConsumerSettings.LockDuration(), nameof(busConsumerSettings.LockDuration), out var ts);
            yield return $@"{configVarName}.LockDuration = TimeSpan.Parse(""{ts}"");";
        }

        yield return $@"{configVarName}.RequiresDuplicateDetection = {busConsumerSettings.RequiresDuplicateDetection().ToString().ToLower()};";
        if (busConsumerSettings.RequiresDuplicateDetection() && !string.IsNullOrWhiteSpace(busConsumerSettings.DuplicateDetectionHistoryTimeWindow()))
        {
            ValidateTimeSpanString(busConsumerSettings.DuplicateDetectionHistoryTimeWindow(), nameof(busConsumerSettings.DuplicateDetectionHistoryTimeWindow), out var ts);
            yield return $@"{configVarName}.DuplicateDetectionHistoryTimeWindow = TimeSpan.Parse(""{ts}"");";
        }

        yield return $@"{configVarName}.EnableBatchedOperations = {busConsumerSettings.EnableBatchedOperations().ToString().ToLower()};";
        yield return $@"{configVarName}.EnableDeadLetteringOnMessageExpiration = {busConsumerSettings.EnableDeadLetteringOnMessageExpiration().ToString().ToLower()};";
        if (busConsumerSettings.MaxQueueSize().HasValue)
        {
            yield return $@"{configVarName}.MaxSizeInMegabytes = {busConsumerSettings.MaxQueueSize()};";
        }

        if (busConsumerSettings.MaxDeliveryCount().HasValue)
        {
            yield return $@"{configVarName}.MaxDeliveryCount = {busConsumerSettings.MaxDeliveryCount()};";
        }
    }

    public override CSharpInvocationStatement AddMessageBrokerConfiguration(string busRegistrationVarName, IEnumerable<CSharpStatement> moreConfiguration)
    {
        var stmt = new CSharpInvocationStatement($"{busRegistrationVarName}.UsingAzureServiceBus")
            .AddArgument(new CSharpLambdaBlock("(context, cfg)")
                .AddInvocationStatement("cfg.Host", host => host
                    .AddArgument(@"configuration[""AzureMessageBus:ConnectionString""]")
                    .SeparatedFromPrevious())
                .AddStatements(moreConfiguration));
        stmt.AddMetadata("message-broker", "azure-service-bus");
        return stmt;
    }

    public override AppSettingRegistrationRequest? GetAppSettings()
    {
        return new AppSettingRegistrationRequest("AzureMessageBus", new
        {
            ConnectionString = "your connection string"
        });
    }

    public override INugetPackageInfo? GetNugetDependency()
    {
        return NuGetPackages.MassTransitAzureServiceBusCore;
    }

    private static void ValidateTimeSpanString(string settingStringValue, string memberName, out TimeSpan parsedTimeSpan)
    {
        if (!TimeSpan.TryParse(settingStringValue, out parsedTimeSpan))
        {
            throw new Exception($"Unable to parse '{settingStringValue}' for {memberName}. Ensure format is 'hh:mm:ss'.");
        }
    }
}