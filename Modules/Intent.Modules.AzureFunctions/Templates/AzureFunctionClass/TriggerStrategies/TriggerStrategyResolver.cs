using System;
using Intent.AzureFunctions.Api;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;

internal static class TriggerStrategyResolver
{
    public static IFunctionTriggerHandler GetFunctionTriggerHandler(AzureFunctionClassTemplate template, IAzureFunctionModel model)
    {
        return model.TriggerType switch
        {
            TriggerType.HttpTrigger => new HttpFunctionTriggerHandler(template, model),
            TriggerType.ServiceBusTrigger => new AzureServiceBusTriggerHandler(template, model),
            TriggerType.QueueTrigger => new QueueTriggerHandler(template, model),
            TriggerType.TimerTrigger => new TimerTriggerHandler(template, model),
            TriggerType.EventHubTrigger => new EventHubTriggerHandler(template, model),
            TriggerType.ManualTrigger => new ManualTriggerHandler(template, model),
            TriggerType.CosmosDBTrigger => new CosmosDBTriggerHandler(template, model),
            TriggerType.RabbitMQTrigger => new RabbitMQTriggerHandler(template, model),
            _ => throw new ArgumentOutOfRangeException($"Unknown trigger type: {model.TriggerType}")
        };
    }
}