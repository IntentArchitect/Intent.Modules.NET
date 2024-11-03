using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;

public class RabbitMQTriggerHandler : IFunctionTriggerHandler
{
    private readonly AzureFunctionClassTemplate _template;
    private readonly IAzureFunctionModel _azureFunctionModel;

    public RabbitMQTriggerHandler(AzureFunctionClassTemplate template, IAzureFunctionModel azureFunctionModel)
    {
        _template = template;
        _azureFunctionModel = azureFunctionModel;
    }

    public IEnumerable<INugetPackageInfo> GetNugetDependencies()
    {
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
                yield return NugetPackages.MicrosoftAzureWebJobsExtensionsRabbitMQ(_template.OutputTarget);
                break;
            default:
            case AzureFunctionsHelper.AzureFunctionsProcessType.Isolated:
                yield return NugetPackages.MicrosoftAzureFunctionsWorkerExtensionsRabbitMQ(_template.OutputTarget);
                break;
        }
    }

    public void ApplyMethodParameters(CSharpClassMethod method)
    {
        if (_azureFunctionModel.Parameters.Count == 0)
        {
            throw new Exception($"Please specify the payload parameter for the RabbitMQ triggered Azure Function [{_azureFunctionModel.Name}]");
        }

        if (_azureFunctionModel.Parameters.Count > 1)
        {
            throw new Exception($"Please specify only one parameter for the RabbitMQ triggered Azure Function [{_azureFunctionModel.Name}]");
        }

        string messageType = _template.GetTypeName(_azureFunctionModel.Parameters.Single().TypeReference);
        string parameterName = _azureFunctionModel.Parameters.Single().Name.ToParameterName();
        if (_azureFunctionModel.IncludeMessageEnvelope)
        {
            messageType = _template.UseType("RabbitMQ.Client.Events.BasicDeliverEventArgs");
            parameterName = "args";

            if (AzureFunctionsHelper.GetAzureFunctionsProcessType(_template.OutputTarget) == AzureFunctionsHelper.AzureFunctionsProcessType.Isolated)
            {
                _template.AddNugetDependency(NugetPackages.RabbitMQClient(_template.OutputTarget));
            }
        }
        
        var rabbitMQTrigger = AzureFunctionsHelper.GetAzureFunctionsProcessType(_template.OutputTarget) == 
                              AzureFunctionsHelper.AzureFunctionsProcessType.InProcess
            ? _template.UseType("Microsoft.Azure.WebJobs.RabbitMQTrigger")
            : _template.UseType("Microsoft.Azure.Functions.Worker.RabbitMQTrigger");
        
        method.AddParameter(
            type: messageType,
            name: parameterName,
            configure: param =>
            {
                param.AddAttribute(rabbitMQTrigger, attr =>
                {
                    attr.AddArgument($@"""{_azureFunctionModel.QueueName}""");
                    if (!string.IsNullOrEmpty(_azureFunctionModel.Connection))
                    {
                        attr.AddArgument($@"ConnectionStringSetting = ""{_azureFunctionModel.Connection}""");
                        _template.ApplyAppSetting(_azureFunctionModel.Connection, "amqp://guest:guest@localhost:5672");
                    }
                });
            });

        method.AddOptionalCancellationTokenParameter();
    }

    public void ApplyMethodStatements(CSharpClassMethod method)
    {
        var messageType = _template.GetTypeName(_azureFunctionModel.Parameters.Single().TypeReference);
        var parameterName = _azureFunctionModel.Parameters.Single().Name.ToParameterName();

        if (_azureFunctionModel.IncludeMessageEnvelope)
        {
            method.AddStatement($"var {parameterName} = {_template.UseType("System.Text.Json.JsonSerializer")}.Deserialize<{messageType}>(args.Body.ToArray());");
        }
    }
}