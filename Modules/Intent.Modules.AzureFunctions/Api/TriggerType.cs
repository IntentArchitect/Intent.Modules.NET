namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;

public enum TriggerType
{
    HttpTrigger,
    ServiceBusTrigger,
    QueueTrigger,
    TimerTrigger,
    EventHubTrigger,
    ManualTrigger,
    CosmosDBTrigger,
    RabbitMQTrigger
}