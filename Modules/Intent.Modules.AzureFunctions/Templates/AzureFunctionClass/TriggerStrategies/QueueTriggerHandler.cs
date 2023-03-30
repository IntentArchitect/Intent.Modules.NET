using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;

internal class QueueTriggerHandler : IFunctionTriggerHandler
{
    private readonly AzureFunctionClassTemplate _template;
    private readonly AzureFunctionModel _azureFunctionModel;

    public QueueTriggerHandler(AzureFunctionClassTemplate template, AzureFunctionModel azureFunctionModel)
    {
        _template = template;
        _azureFunctionModel = azureFunctionModel;
    }

    public void ApplyMethodParameters(CSharpClassMethod method)
    {
        method.AddParameter(
            type: _template.GetTypeName(_azureFunctionModel.Parameters.Single().TypeReference),
            name: _azureFunctionModel.Parameters.Single().Name.ToParameterName(),
            configure: param =>
            {
                param.AddAttribute("QueueTrigger", attr =>
                {
                    var serviceBusTriggerView = _azureFunctionModel.GetAzureFunction().GetQueueTriggerView();
                    attr.AddArgument($@"""{serviceBusTriggerView.QueueName()}""");
                    if (!string.IsNullOrEmpty(serviceBusTriggerView.Connection()))
                    {
                        attr.AddArgument($@"Connection = ""{serviceBusTriggerView.Connection()}""");
                    }
                });
            });
    }

    public void ApplyMethodStatements(CSharpClassMethod method)
    {
    }

    public IEnumerable<INugetPackageInfo> GetNugetDependencies()
    {
        yield return NuGetPackages.MicrosoftAzureWebJobsExtensionsStorageQueues;
    }
}