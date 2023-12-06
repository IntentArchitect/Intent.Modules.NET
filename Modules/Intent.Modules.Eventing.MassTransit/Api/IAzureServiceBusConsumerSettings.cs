namespace Intent.Eventing.MassTransit.Api;

public interface IAzureServiceBusConsumerSettings
{
    string Name { get; }
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
}