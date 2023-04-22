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
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}