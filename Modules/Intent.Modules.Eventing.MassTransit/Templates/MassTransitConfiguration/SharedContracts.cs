using Intent.Eventing.MassTransit.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;

public record Consumer
{
    public MessageDetail Message { get; init; }
    public ConsumerSettings Settings { get; init; } = new ConsumerSettings();
    public string ConsumerTypeName { get; init; }
    public string ConsumerDefinitionTypeName { get; init; }
    public bool IsSpecificMessageConsumer { get; init; }
    public string? DestinationAddress { get; init; }
}

public record MessageDetail
{
    public static MessageDetail CreateFrom<TModel>(MessageModel messageModel, CSharpTemplateBase<TModel> template)
    {
        return new MessageDetail
        {
            MessageName = messageModel.Name,
            MessageTypeFullName = template.GetFullyQualifiedTypeName(messageModel.InternalElement),
            TopicNameOverride = messageModel.GetMessageTopologySettings()?.EntityName()
        };
    }
    
    public string MessageName { get; init; }
    public string MessageTypeFullName { get; init; }
    public string? TopicNameOverride { get; init; }
}

public record ConsumerSettings
{
    public IAzureServiceBusConsumerSettings? AzureConsumerSettings { get; init; }
    public IRabbitMQConsumerSettings? RabbitMqConsumerSettings { get; init; }
}

public record Producer
{
    public string MessageTypeName { get; init; }
    public string Urn { get; init; }
}