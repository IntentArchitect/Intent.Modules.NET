using Intent.Modules.Common.Templates;

namespace Intent.Modules.Integration.IaC.Shared.AzureServiceBus;

internal abstract record AzureServiceBusItemBase
{
    public string ApplicationId { get; init; }
    public string ApplicationName { get; init; }
    public AzureServiceBusMethodType MethodType { get; init; }
    public AzureServiceBusChannelType ChannelType { get; init; }
    public string QueueOrTopicName { get; init; }
    public string QueueOrTopicConfigurationName { get; init; }
    public string? QueueOrTopicSubscriptionConfigurationName { get; init; }

    public abstract string GetModelTypeName(IntentTemplateBase template);
    public abstract string GetSubscriberTypeName<T>(IntentTemplateBase<T> template);
};