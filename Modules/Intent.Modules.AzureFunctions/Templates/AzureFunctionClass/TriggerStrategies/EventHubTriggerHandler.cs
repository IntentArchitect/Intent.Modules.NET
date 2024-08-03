using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;

internal class EventHubTriggerHandler : IFunctionTriggerHandler
{
    private readonly AzureFunctionClassTemplate _template;
    private readonly IAzureFunctionModel _azureFunctionModel;

    public EventHubTriggerHandler(AzureFunctionClassTemplate template, IAzureFunctionModel azureFunctionModel)
    {
        _template = template;
        _azureFunctionModel = azureFunctionModel;
    }

    public void ApplyMethodParameters(CSharpClassMethod method)
    {
        if (_azureFunctionModel.Parameters.Count == 0)
        {
            throw new Exception($"Please specify the parameter for the EventHub triggered Azure Function [{_azureFunctionModel.Name}]");
        }

        if (_azureFunctionModel.Parameters.Count > 1)
        {
            throw new Exception($"Please specify only one parameter for the EventHub triggered Azure Function [{_azureFunctionModel.Name}]");
        }

        method.AddParameter(
            type: _azureFunctionModel.Parameters.Any() ? _template.GetTypeName(_azureFunctionModel.Parameters.Single().TypeReference) : _template.UseType("Azure.Messaging.EventHubs.EventData"),
            name: _azureFunctionModel.Parameters.Any() ? _azureFunctionModel.Parameters.Single().Name.ToParameterName() : "eventData",
            configure: param =>
            {
                param.AddAttribute(_template.UseType("Microsoft.Azure.WebJobs.EventHubTrigger"), attr =>
                {
                    attr.AddArgument($@"""{_azureFunctionModel.EventHubName}""");
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
        if (!_azureFunctionModel.Parameters.Any())
        {
            yield return NugetPackages.AzureMessagingEventHubs(_template.OutputTarget);
        }
        yield return NugetPackages.MicrosoftAzureWebJobsExtensionsEventHubs(_template.OutputTarget);
    }
}