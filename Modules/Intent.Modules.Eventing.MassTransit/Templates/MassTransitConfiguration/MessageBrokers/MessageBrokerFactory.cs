using System;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Eventing.MassTransit.Settings;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.MessageBrokers;

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