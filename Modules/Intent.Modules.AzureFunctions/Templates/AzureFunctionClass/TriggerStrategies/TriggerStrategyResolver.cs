using System;
using Intent.AzureFunctions.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;

internal static class TriggerStrategyResolver
{
    public static IFunctionTriggerHandler GetFunctionTriggerHandler(AzureFunctionClassTemplate template, AzureFunctionModel model)
    {
        switch (model.GetAzureFunction()?.Type()?.AsEnum())
        {
            case AzureFunctionModelStereotypeExtensions.AzureFunction.TypeOptionsEnum.HttpTrigger:
                return new HttpFunctionTriggerHandler(template, model);
            case AzureFunctionModelStereotypeExtensions.AzureFunction.TypeOptionsEnum.ServiceBusTrigger:
                return new AzureServiceBusTriggerHandler(template, model);
            case AzureFunctionModelStereotypeExtensions.AzureFunction.TypeOptionsEnum.QueueTrigger:
                return new QueueTriggerHandler(template, model);
            case null:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}