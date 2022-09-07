using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;

public class QueueTriggerHandler : IFunctionTriggerHandler
{
    private readonly AzureFunctionClassTemplate _template;
    private readonly OperationModel _operationModel;

    public QueueTriggerHandler(AzureFunctionClassTemplate template, OperationModel operationModel)
    {
        _template = template;
        _operationModel = operationModel;
    }

    public IEnumerable<string> GetMethodParameterDefinitionList()
    {
        var paramList = new List<string>();
        var serviceBusTriggerView = _operationModel.GetAzureFunction().GetQueueTriggerView();
        var attrParamList = new List<string>();
        attrParamList.Add($@"""{serviceBusTriggerView.QueueName()}""");
        if (!string.IsNullOrEmpty(serviceBusTriggerView.Connection()))
        {
            attrParamList.Add($@"Connection = ""{serviceBusTriggerView.Connection()}""");
        }
        
        paramList.Add(
            $@"[QueueTrigger({string.Join(", ", attrParamList)})] {GetRequestType()} {GetRequestParameterName()}");

        return paramList;
    }

    public IEnumerable<string> GetRunMethodEntryStatementList()
    {
        yield break;
    }

    public IEnumerable<INugetPackageInfo> GetNugetDependencies()
    {
        yield return NuGetPackages.MicrosoftAzureWebJobsExtensionsStorageQueues;
    }

    public IEnumerable<ExceptionCatchBlock> GetExceptionCatchBlocks()
    {
        yield break;
    }
    
    private string GetRequestParameterName()
    {
        return _operationModel.Parameters.Single().Name.ToParameterName();
    }

    private string GetRequestType()
    {
        return _template.GetTypeName(_operationModel.Parameters.Single().TypeReference);
    }
}