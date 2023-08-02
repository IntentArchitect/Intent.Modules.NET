using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;

internal class QueueTriggerHandler : IFunctionTriggerHandler
{
    private readonly AzureFunctionClassTemplate _template;
    private readonly IAzureFunctionModel _azureFunctionModel;

    public QueueTriggerHandler(AzureFunctionClassTemplate template, IAzureFunctionModel azureFunctionModel)
    {
        _template = template;
        _azureFunctionModel = azureFunctionModel;
    }

    public void ApplyMethodParameters(CSharpClassMethod method)
    {
        if (_azureFunctionModel.Parameters.Count == 0)
        {
            throw new Exception($"Please specify the parameter for the Queue triggered Azure Function [{_azureFunctionModel.Name}]");
        }

        if (_azureFunctionModel.Parameters.Count > 1)
        {
            throw new Exception($"Please specify only one parameter for the Queue triggered Azure Function [{_azureFunctionModel.Name}]");
        }

        string messageType = _template.GetTypeName(_azureFunctionModel.Parameters.Single().TypeReference);
        string parameterName = _azureFunctionModel.Parameters.Single().Name.ToParameterName();
        if (!string.IsNullOrEmpty(_azureFunctionModel.MessageType) && _azureFunctionModel.MessageType == "QueueMessage")
        {
            messageType = _template.UseType("Azure.Storage.Queues.Models.QueueMessage");
            parameterName = "message";
        }

        method.AddParameter(
            type: messageType,
            name: parameterName,
            configure: param =>
            {
                param.AddAttribute("QueueTrigger", attr =>
                {
                    attr.AddArgument($@"""{_azureFunctionModel.QueueName}""");
                    if (!string.IsNullOrEmpty(_azureFunctionModel.Connection))
                    {
                        attr.AddArgument($@"Connection = ""{_azureFunctionModel.Connection}""");
                        _template.ApplyAppSetting(_azureFunctionModel.Connection, "UseDevelopmentStorage=true");
                    }
                });
            });

        if (_azureFunctionModel.InternalElement.HasStereotype("Queue Output Binding"))
        {
            _template.AddTypeSource("Intent.Application.MediatR.CommandModels");
            foreach (var outputBinding in _azureFunctionModel.InternalElement.GetStereotypes("Queue Output Binding"))
            {
                string bindingType = outputBinding.GetProperty<string>("Type");
                switch (bindingType)
                {
                    case "ICollector<T>":
                    case "Message":
                        method.Sync();
                        break;
                    default:
                            break;
                }

                method.AddParameter(
                    type: GetOutputBindingParameterType(outputBinding),
                    name: outputBinding.GetProperty<string>("Parameter Name").ToParameterName(),
                    configure: param =>
                    {
                        if (bindingType == "Message")
                        {
                            param.WithOutParameterModifier();
                        }
                        param.AddAttribute("Queue", attr =>
                        {
                            attr.AddArgument($@"""{outputBinding.GetProperty<string>("Queue Name")}""");
                        });
                    });
            }
        }

        method.AddParameter(_template.UseType("System.Threading.CancellationToken"), "cancellationToken");
    }

    private string GetOutputBindingParameterType(IStereotype outputBinding)
    {
        string bindingType = outputBinding.GetProperty<string>("Type");
        switch (bindingType)
        {
            case "ICollector<T>":
                return $"ICollector<{GetMessageType(outputBinding)}>";
            case "IAsyncCollector<T>":
                return $"IAsyncCollector<{GetMessageType(outputBinding)}>";
            case "QueueClient":
                return _template.UseType("Azure.Storage.Queues.QueueClient");
            case "Message":
            default:
                return GetMessageType(outputBinding);
        }
    }

    private string GetMessageType(IStereotype outputBinding)
    {
        return _template.GetTypeName(outputBinding.GetProperty<IElement>("Message Type"));
    }

    public void ApplyMethodStatements(CSharpClassMethod method)
    {
        string messageType = _template.GetTypeName(_azureFunctionModel.Parameters.Single().TypeReference);
        string parameterName = _azureFunctionModel.Parameters.Single().Name.ToParameterName();

        if (!string.IsNullOrEmpty(_azureFunctionModel.MessageType) && _azureFunctionModel.MessageType == "QueueMessage")
        {
            method.AddStatement($"var {parameterName} = {_template.UseType("Newtonsoft.Json.JsonConvert")}.DeserializeObject<{messageType}>(message.Body.ToString());");
        }

        if (_azureFunctionModel.InternalElement.HasStereotype("Queue Output Binding"))
        {
            method.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
            method.AddStatement("throw new NotImplementedException(\"Your custom logic here...\");");
        }
    }

    public IEnumerable<INugetPackageInfo> GetNugetDependencies()
    {
        yield return NuGetPackages.MicrosoftAzureWebJobsExtensionsStorageQueues;
    }
}