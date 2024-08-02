using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

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
        if (_azureFunctionModel.IncludeMessageEnvelope)
        {
            messageType = _template.UseType("Azure.Storage.Queues.Models.QueueMessage");
            parameterName = "rawMessage";
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

        if (_azureFunctionModel.ReturnType != null)
        {
            if (((IElement)_azureFunctionModel.ReturnType.Element).SpecializationType == "Command")
            {
                _template.AddTypeSource("Intent.Application.MediatR.CommandModels");
            }
            method.AddParameter(
                type: _template.UseType("Azure.Storage.Queues.QueueClient"),
                name: "queueClient",
                configure: param =>
                {
                    if (_azureFunctionModel.InternalElement.HasStereotype("Queue Output Binding"))
                    {
                        var outputBinding = _azureFunctionModel.InternalElement.GetStereotype("Queue Output Binding");
                        param.AddAttribute("Queue", attr =>
                        {
                            attr.AddArgument($@"""{outputBinding.GetProperty<string>("Queue Name")}""");
                        });
                    }
                });

            //Doing this for operations and commands , not AzureFunctionModels as they don't have implementations
            if (_template.Model.InternalElement.AsAzureFunctionModel() == null)
            {
                _template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var runMethod = @class.FindMethod("Run");
                    
                    var returnStatement = runMethod?.FindStatement(x => x.HasMetadata("return"));
                    returnStatement?.Remove();
                    runMethod?.AddStatement($"await queueClient.SendMessageAsync({_template.UseType("System.Text.Json.JsonSerializer")}.Serialize(result), cancellationToken);",
                        stmt => stmt.AddMetadata("return", true));
                }, 100);
            }
        }

        method.AddParameter(_template.UseType("System.Threading.CancellationToken"), "cancellationToken");
    }

    public void ApplyMethodStatements(CSharpClassMethod method)
    {
        string messageType = _template.GetTypeName(_azureFunctionModel.Parameters.Single().TypeReference);
        string parameterName = _azureFunctionModel.Parameters.Single().Name.ToParameterName();

        if (_azureFunctionModel.IncludeMessageEnvelope)
        {
            method.AddStatement($"var {parameterName} = {_template.UseType("System.Text.Json.JsonSerializer")}.Deserialize<{messageType}>(rawMessage.Body.ToString(), new JsonSerializerOptions {{PropertyNameCaseInsensitive = true}})!;");
        }
    }

    public IEnumerable<INugetPackageInfo> GetNugetDependencies()
    {
        yield return NugetPackages.MicrosoftAzureWebJobsExtensionsStorageQueues(_template.OutputTarget);
    }
}