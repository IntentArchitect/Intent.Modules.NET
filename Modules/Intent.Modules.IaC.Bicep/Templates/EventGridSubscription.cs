using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.IaC.Bicep.Templates;

internal record EventGridSubscription
{
    public EventGridSubscription(InfrastructureRegisteredEvent @event)
    {
        FullyQualifiedEventNames = @event.Properties[Infrastructure.AzureEventGrid.Property.MessageNames];
        HandlerFunctionName = @event.Properties[Infrastructure.AzureEventGrid.Property.HandlerFunctionName];
        TopicName = @event.Properties[Infrastructure.AzureEventGrid.Property.TopicName].ToKebabCase();
    }

    public string FullyQualifiedEventNames { get; }
    public string HandlerFunctionName { get; }
    public string TopicName { get; }
    public string TopicResourceName => $"eventGridTopic{TopicName.ToPascalCase()}";
    public string SubscriptionResourceName => $"eventGridSubscription{TopicName.ToPascalCase()}";
}