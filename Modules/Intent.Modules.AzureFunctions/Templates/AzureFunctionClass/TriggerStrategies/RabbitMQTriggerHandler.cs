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
        yield return NugetPackages.MicrosoftAzureWebJobsExtensionsRabbitMQ(_template.OutputTarget);
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
        }
        
        method.AddParameter(
            type: messageType,
            name: parameterName,
            configure: param =>
            {
                param.AddAttribute(_template.UseType("Microsoft.Azure.WebJobs.RabbitMQTrigger"), attr =>
                {
                    attr.AddArgument($@"""{_azureFunctionModel.QueueName}""");
                    if (!string.IsNullOrEmpty(_azureFunctionModel.Connection))
                    {
                        attr.AddArgument($@"ConnectionStringSetting = ""{_azureFunctionModel.Connection}""");
                        _template.ApplyAppSetting(_azureFunctionModel.Connection, "amqp://guest:guest@localhost:5672");
                    }
                });
            });
        
        method.AddParameter(_template.UseType("System.Threading.CancellationToken"), "cancellationToken");
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