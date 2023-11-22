using System;
using Intent.AzureFunctions.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;

internal static class TriggerStrategyResolver
{
    public static IFunctionTriggerHandler GetFunctionTriggerHandler(AzureFunctionClassTemplate template, IAzureFunctionModel model)
    {
        switch (model.TriggerType)
        {
            case TriggerType.HttpTrigger:
                return new HttpFunctionTriggerHandler(template, model);
            case TriggerType.ServiceBusTrigger:
                return new AzureServiceBusTriggerHandler(template, model);
            case TriggerType.QueueTrigger:
                return new QueueTriggerHandler(template, model);
            case TriggerType.TimerTrigger:
                return new TimerTriggerHandler(template, model);
            case TriggerType.EventHubTrigger:
                return new EventHubTriggerHandler(template, model);
            case TriggerType.ManualTrigger:
                return new ManualTriggerHandler(template, model);
            case TriggerType.CosmosDBTrigger:
                return new CosmosDBTriggerHandler(template, model);
            case TriggerType.RabbitMQTrigger:
                return new RabbitMQTriggerHandler(template, model);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}