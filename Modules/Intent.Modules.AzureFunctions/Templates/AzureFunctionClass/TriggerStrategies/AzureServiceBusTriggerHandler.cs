using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;

internal class AzureServiceBusTriggerHandler : IFunctionTriggerHandler
{
    private readonly AzureFunctionClassTemplate _template;
    private readonly IAzureFunctionModel _azureFunctionModel;

    public AzureServiceBusTriggerHandler(AzureFunctionClassTemplate template, IAzureFunctionModel azureFunctionModel)
    {
        _template = template;
        _azureFunctionModel = azureFunctionModel;
    }

    public void ApplyMethodParameters(CSharpClassMethod method)
    {
        if (_azureFunctionModel.Parameters.Count == 0)
        {
            throw new Exception($"Please specify the parameter for the ServiceBus triggered Azure Function [{_azureFunctionModel.Name}]");
        }

        if (_azureFunctionModel.Parameters.Count > 1)
        {
            throw new Exception($"Please specify only one parameter for the ServiceBus triggered Azure Function [{_azureFunctionModel.Name}]");
        }

        var parameter = _azureFunctionModel.Parameters.Single();
        string typeName = _template.GetTypeName(parameter.TypeReference);
        var isTopic = !string.IsNullOrWhiteSpace(_azureFunctionModel.SubscriptionName);
        method.AddParameter(
            type: typeName,
            name: parameter.Name.ToParameterName(),
            configure: param =>
            {
                param.AddAttribute("ServiceBusTrigger", attr =>
                {
                    if (TriggerAppSettings.IsEnabled(_template))
                    {
                        // The app setting needs a concrete default to seed, so a blank Queue Name falls back to the
                        // message type name (what nameof(...) evaluates to when literal names are used).
                        var queueOrTopicName = string.IsNullOrWhiteSpace(_azureFunctionModel.QueueName)
                            ? parameter.TypeReference.Element.Name
                            : _azureFunctionModel.QueueName;
                        attr.AddArgument(TriggerAppSettings.GetBindingValue(_template, isTopic ? "Topic" : "Queue", queueOrTopicName));
                    }
                    else
                    {
                        attr.AddArgument(string.IsNullOrWhiteSpace(_azureFunctionModel.QueueName) ? $"nameof({typeName})" : $@"""{_azureFunctionModel.QueueName}""");
                    }
                    if (isTopic)
                    {
                        attr.AddArgument(TriggerAppSettings.GetBindingValue(_template, "Subscription", _azureFunctionModel.SubscriptionName));
                    }
                    if (!string.IsNullOrEmpty(_azureFunctionModel.Connection))
                    {
                        attr.AddArgument($@"Connection = ""{_azureFunctionModel.Connection}""");
                        TriggerAppSettings.SeedConnection(_template, _azureFunctionModel.Connection);
                    }
                });
            });
        method.AddParameter(_template.UseType("System.Threading.CancellationToken"), "cancellationToken");
    }

    public void ApplyMethodStatements(CSharpClassMethod method)
    {
    }

    public IEnumerable<INugetPackageInfo> GetNugetDependencies()
    {
        yield return NugetPackages.MicrosoftAzureServiceBus(_template.OutputTarget);

        foreach (var nugetPackageInfo in GetNetSpecificPackages(AzureFunctionsHelper.GetAzureFunctionsProcessType(_template.OutputTarget)))
        {
            yield return nugetPackageInfo;
        }
    }

    public IEnumerable<INugetPackageInfo> GetNugetRedundantDependencies()
    {
        foreach (var nugetPackageInfo in GetNetSpecificPackages(AzureFunctionsHelper.GetAzureFunctionsProcessType(_template.OutputTarget).SwapState()))
        {
            yield return nugetPackageInfo;
        }
    }
    
    private IEnumerable<INugetPackageInfo> GetNetSpecificPackages(AzureFunctionsHelper.AzureFunctionsProcessType azureFunctionsProcessType)
    {
        switch (azureFunctionsProcessType)
        {
            case AzureFunctionsHelper.AzureFunctionsProcessType.InProcess:
                yield return NugetPackages.MicrosoftAzureWebJobsExtensionsServiceBus(_template.OutputTarget);
                break;
            default:
            case AzureFunctionsHelper.AzureFunctionsProcessType.Isolated:
                yield return NugetPackages.MicrosoftAzureFunctionsWorkerExtensionsServiceBus(_template.OutputTarget);
                break;
        }
    }
}
