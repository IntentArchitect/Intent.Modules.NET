using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Eventing.MassTransit.Settings;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.MessageBrokers;

internal class AzureServiceBusMessageBroker : MessageBrokerBase
{
    public AzureServiceBusMessageBroker(ICSharpFileBuilderTemplate template) : base(template)
    {
    }

    public override string GetMessageBrokerBusFactoryConfiguratorName()
    {
        return "IServiceBusBusFactoryConfigurator";
    }

    public override CSharpInvocationStatement AddMessageBrokerConfiguration(string busRegistrationVarName, string factoryConfigVarName,
        IEnumerable<CSharpStatement> moreConfiguration)
    {
        var stmt = new CSharpInvocationStatement($"{busRegistrationVarName}.UsingAzureServiceBus")
            .AddArgument(new CSharpLambdaBlock($"(context, {factoryConfigVarName})")
                .AddInvocationStatement($"{factoryConfigVarName}.Host", host => host
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

    public override INugetPackageInfo? GetNugetDependency(IOutputTarget outputTarget)
    {
        return NugetPackages.MassTransitAzureServiceBusCore(outputTarget);
    }

    public override IEnumerable<CSharpStatement> GetCustomConfigurationStatements(Consumer consumer, string sanitizedAppName)
    {
        if (consumer.Settings.AzureConsumerSettings!.EndpointTypeSelection().IsReceiveEndpoint())
        {
            CSharpLambdaBlock? definitionBlock = null;
            if (!string.IsNullOrWhiteSpace(consumer.Settings.AzureConsumerSettings.EndpointName()))
            {
                definitionBlock = new CSharpLambdaBlock("definition");
                definitionBlock.WithExpressionBody($@"definition.Name = ""{consumer.Settings.AzureConsumerSettings.EndpointName()}""");
            }

            yield return CreateConsumerReceiveEndpointStatement(consumer, sanitizedAppName, definitionBlock,
                GetConfigurationStatements("endpoint", consumer), _template);
        }

        if (consumer.Settings.AzureConsumerSettings!.EndpointTypeSelection().IsSubscriptionEndpoint())
        {
            var hasTopicNameOverride = !string.IsNullOrWhiteSpace(consumer.Message.TopicNameOverride);
            var genericParamInvocation = !hasTopicNameOverride ? $"<{_template.UseType(consumer.Message.MessageTypeFullName)}>" : string.Empty;

            var subscription = new CSharpInvocationStatement($"cfg.SubscriptionEndpoint{genericParamInvocation}");
            subscription.AddArgument($@"""{consumer.Settings.AzureConsumerSettings.EndpointName()}""");

            if (hasTopicNameOverride)
            {
                subscription.AddArgument($@"""{consumer.Message.TopicNameOverride}""");
            }

            var endpointsBlock = new CSharpLambdaBlock("endpoint")
                .AddStatements(GetConfigurationStatements("endpoint", consumer))
                .AddStatement($@"endpoint.ConfigureConsumer<{consumer.ConsumerTypeName}>(context);");
            
            subscription.AddArgument(endpointsBlock);
            
            yield return subscription;
        }
    }

    public override IEnumerable<CSharpClassMethod> GetCustomConfigurationHelperMethods(CSharpClass configurationClass)
    {
        yield return CreateConsumerReceiveEndpointMethod(configurationClass, GetMessageBrokerBusFactoryConfiguratorName(),
            "IServiceBusReceiveEndpointConfigurator");
    }

    private IEnumerable<CSharpStatement> GetConfigurationStatements(string configVarName, Consumer consumer)
    {
        var busConsumerSettings = consumer.Settings.AzureConsumerSettings!;

        if (busConsumerSettings.PrefetchCount().HasValue)
        {
            yield return $@"{configVarName}.PrefetchCount = {busConsumerSettings.PrefetchCount()};";
        }

        yield return $@"{configVarName}.RequiresSession = {busConsumerSettings.RequiresSession().ToString().ToLower()};";
        if (!string.IsNullOrWhiteSpace(busConsumerSettings.DefaultMessageTimeToLive()))
        {
            ValidateTimeSpanString(busConsumerSettings.DefaultMessageTimeToLive(), nameof(busConsumerSettings.DefaultMessageTimeToLive));
            yield return $@"{configVarName}.DefaultMessageTimeToLive = TimeSpan.Parse(""{busConsumerSettings.DefaultMessageTimeToLive()}"");";
        }

        if (!string.IsNullOrWhiteSpace(busConsumerSettings.LockDuration()))
        {
            ValidateTimeSpanString(busConsumerSettings.LockDuration(), nameof(busConsumerSettings.LockDuration));
            yield return $@"{configVarName}.LockDuration = TimeSpan.Parse(""{busConsumerSettings.LockDuration()}"");";
        }

        if (busConsumerSettings.EndpointTypeSelection().IsReceiveEndpoint())
        {
            yield return $@"{configVarName}.RequiresDuplicateDetection = {busConsumerSettings.RequiresDuplicateDetection().ToString().ToLower()};";
            if (busConsumerSettings.RequiresDuplicateDetection() && !string.IsNullOrWhiteSpace(busConsumerSettings.DuplicateDetectionHistoryTimeWindow()))
            {
                ValidateTimeSpanString(busConsumerSettings.DuplicateDetectionHistoryTimeWindow(), nameof(busConsumerSettings.DuplicateDetectionHistoryTimeWindow));
                yield return $@"{configVarName}.DuplicateDetectionHistoryTimeWindow = TimeSpan.Parse(""{busConsumerSettings.DuplicateDetectionHistoryTimeWindow()}"");";
            }
        }

        yield return $@"{configVarName}.EnableBatchedOperations = {busConsumerSettings.EnableBatchedOperations().ToString().ToLower()};";
        yield return $@"{configVarName}.EnableDeadLetteringOnMessageExpiration = {busConsumerSettings.EnableDeadLetteringOnMessageExpiration().ToString().ToLower()};";

        if (busConsumerSettings.EndpointTypeSelection().IsReceiveEndpoint() && busConsumerSettings.MaxQueueSize().HasValue)
        {
            yield return $@"{configVarName}.MaxSizeInMegabytes = {busConsumerSettings.MaxQueueSize()};";
        }

        if (busConsumerSettings.MaxDeliveryCount().HasValue)
        {
            yield return $@"{configVarName}.MaxDeliveryCount = {busConsumerSettings.MaxDeliveryCount()};";
        }

        if (busConsumerSettings.ConcurrentMessageLimit().HasValue)
        {
            yield return $@"{configVarName}.ConcurrentMessageLimit = {busConsumerSettings.ConcurrentMessageLimit()};";
        }

        if (busConsumerSettings.MaxConcurrentCallsPerSession().HasValue)
        {
            yield return $@"{configVarName}.MaxConcurrentCallsPerSession = {busConsumerSettings.MaxConcurrentCallsPerSession()};";
        }
    }

    private static void ValidateTimeSpanString(string settingStringValue, string memberName)
    {
        if (!TimeSpan.TryParse(settingStringValue, out _))
        {
            throw new Exception($"Unable to parse '{settingStringValue}' for {memberName}. Ensure format is 'hh:mm:ss'.");
        }
    }
}