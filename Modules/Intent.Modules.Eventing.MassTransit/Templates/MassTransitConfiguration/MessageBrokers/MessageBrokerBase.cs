using System;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Eventing.MassTransit.Settings;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.MessageBrokers;

internal abstract class MessageBrokerBase
{
    protected readonly ICSharpFileBuilderTemplate _template;

    protected MessageBrokerBase(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public abstract bool HasMessageBrokerStereotype(Subscription subscription);
    public abstract string GetMessageBrokerBusFactoryConfiguratorName();
    public abstract string GetMessageBrokerReceiveEndpointConfiguratorName();
    public abstract IEnumerable<CSharpStatement> AddBespokeConsumerConfigurationStatements(string configVarName, Subscription subscription);
    public abstract CSharpInvocationStatement AddMessageBrokerConfiguration(string busRegistrationVarName, IEnumerable<CSharpStatement> moreConfiguration);
    public abstract AppSettingRegistrationRequest? GetAppSettings();
    public abstract INugetPackageInfo? GetNugetDependency();
}

internal static class MessageBrokerFactory
{
    public static MessageBrokerBase GetMessageBroker(ICSharpFileBuilderTemplate template)
    {
        var messageBrokerSetting = template.ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum();
        return messageBrokerSetting switch
        {
            EventingSettings.MessagingServiceProviderOptionsEnum.InMemory => new InMemoryMessageBroker(template),
            EventingSettings.MessagingServiceProviderOptionsEnum.Rabbitmq => new RabbitMqMessageBroker(template),
            EventingSettings.MessagingServiceProviderOptionsEnum.AzureServiceBus => new AzureServiceBusMessageBroker(template),
            EventingSettings.MessagingServiceProviderOptionsEnum.AmazonSqs => new AmazonSqsMessageBroker(template),
            _ => throw new InvalidOperationException(
                $"Messaging Service Provider is set to a setting that is not supported: {messageBrokerSetting}")
        };
    }
}