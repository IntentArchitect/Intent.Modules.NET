using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
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

        string typeName = _template.GetTypeName(_azureFunctionModel.Parameters.Single().TypeReference);
        method.AddParameter(
            type: typeName,
            name: _azureFunctionModel.Parameters.Single().Name.ToParameterName(),
            configure: param =>
            {
                param.AddAttribute("ServiceBusTrigger", attr =>
                {
                    attr.AddArgument(string.IsNullOrWhiteSpace(_azureFunctionModel.QueueName) ? $"nameof({typeName})" : $@"""{_azureFunctionModel.QueueName}""");
                    if (!string.IsNullOrEmpty(_azureFunctionModel.Connection))
                    {
                        attr.AddArgument($@"Connection = ""{_azureFunctionModel.Connection}""");
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