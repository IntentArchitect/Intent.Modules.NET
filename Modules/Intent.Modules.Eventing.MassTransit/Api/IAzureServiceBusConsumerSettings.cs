using Intent.RoslynWeaver.Attributes;

namespace Intent.Eventing.MassTransit.Api;

// This interface will allow us to seamlessly use any "AzureServiceBusConsumerSettings" stereotype that's been assigned to various elements. 
public interface IAzureServiceBusConsumerSettings
{
    string Name { get; }
    string EndpointName();
    EndpointTypeOptionsAdapted EndpointTypeSelection();
    int? PrefetchCount();
    bool RequiresSession();
    string DefaultMessageTimeToLive();
    string LockDuration();
    bool RequiresDuplicateDetection();
    string DuplicateDetectionHistoryTimeWindow();
    bool EnableBatchedOperations();
    bool EnableDeadLetteringOnMessageExpiration();
    int? MaxQueueSize();
    int? MaxDeliveryCount();
    int? ConcurrentMessageLimit();
    int? MaxConcurrentCallsPerSession();
}

// We need a pattern to properly reuse the same stereotype contracts instead of having to abstract it like this
[IntentIgnore]
public class EndpointTypeOptionsAdapted
{
    private readonly SubscribeIntegrationEventTargetEndModelStereotypeExtensions.AzureServiceBusConsumerSettings.EndpointTypeOptions _original; 

    public EndpointTypeOptionsAdapted(string value)
    {
        _original = new SubscribeIntegrationEventTargetEndModelStereotypeExtensions.AzureServiceBusConsumerSettings.EndpointTypeOptions(value);
    }
    
    public bool IsReceiveEndpoint()
    {
        return _original is { Value: null } /* Compatibility with prev version */ || _original.IsReceiveEndpoint();
    }
    
    public bool IsSubscriptionEndpoint()
    {
        return _original.IsSubscriptionEndpoint();
    }
}