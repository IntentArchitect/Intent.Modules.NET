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
    private readonly IAzureFunctionModel _model;

    public QueueTriggerHandler(AzureFunctionClassTemplate template, IAzureFunctionModel model)
    {
        _template = template;
        _model = model;
    }

    public void ApplyMethodParameters(CSharpClassMethod method)
    {
        if (_model.Parameters.Count == 0)
        {
            throw new Exception($"Please specify the parameter for the Queue triggered Azure Function [{_model.Name}]");
        }

        if (_model.Parameters.Count > 1)
        {
            throw new Exception($"Please specify only one parameter for the Queue triggered Azure Function [{_model.Name}]");
        }

        var messageType = _template.GetTypeName(_model.Parameters.Single().TypeReference);
        var parameterName = _model.Parameters.Single().Name.ToParameterName();
        if (_model.IncludeMessageEnvelope)
        {
            messageType = _template.UseType("Azure.Storage.Queues.Models.QueueMessage");
            parameterName = "rawMessage";
        }

        method.AddParameter(type: messageType, name: parameterName, configure: param => param
            .AddAttribute("QueueTrigger", attr =>
            {
                attr.AddArgument($@"""{_model.QueueName}""");
                if (string.IsNullOrEmpty(_model.Connection))
                {
                    return;
                }
                attr.AddArgument($@"Connection = ""{_model.Connection}""");
                _template.ApplyAppSetting(_model.Connection, "UseDevelopmentStorage=true");
            }));

        if (_model.ReturnType != null && 
            AzureFunctionsHelper.GetAzureFunctionsProcessType(_template.OutputTarget) == AzureFunctionsHelper.AzureFunctionsProcessType.InProcess)
        {
            if (IsCommand(_model.ReturnType))
            {
                _template.AddTypeSource("Intent.Application.MediatR.CommandModels");
            }

            method.AddParameter(type: _template.UseType("Azure.Storage.Queues.QueueClient"), name: "queueClient", configure: param =>
            {
                if (!_model.InternalElement.HasStereotype("Queue Output Binding"))
                {
                    return;
                }
                var outputBinding = _model.InternalElement.GetStereotype("Queue Output Binding");
                param.AddAttribute("Queue", attr => { attr.AddArgument($@"""{outputBinding.GetProperty<string>("Queue Name")}"""); });
            });

            //Doing this for operations and commands , not AzureFunctionModels as they don't have implementations
            if (!_template.Model.InternalElement.IsAzureFunctionModel())
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
        else if (_model.ReturnType != null &&
                 AzureFunctionsHelper.GetAzureFunctionsProcessType(_template.OutputTarget) == AzureFunctionsHelper.AzureFunctionsProcessType.Isolated)
        {
            var outputBinding = _model.InternalElement.GetStereotype("Queue Output Binding");
            method.AddAttribute(_template.UseType("Microsoft.Azure.Functions.Worker.QueueOutput"), attr => attr.AddArgument($@"""{outputBinding.GetProperty<string>("Queue Name")}"""));
        }

        method.AddOptionalCancellationTokenParameter();
        return;

        static bool IsCommand(ITypeReference modelReturnType)
        {
            return ((IElement)modelReturnType.Element).SpecializationType == "Command";
        }
    }

    public void ApplyMethodStatements(CSharpClassMethod method)
    {
        string messageType = _template.GetTypeName(_model.Parameters.Single().TypeReference);
        string parameterName = _model.Parameters.Single().Name.ToParameterName();

        if (_model.IncludeMessageEnvelope)
        {
            method.AddStatement(
                $"var {parameterName} = {_template.UseType("System.Text.Json.JsonSerializer")}.Deserialize<{messageType}>(rawMessage.Body.ToString(), new JsonSerializerOptions {{PropertyNameCaseInsensitive = true}})!;");
        }
    }

    public IEnumerable<INugetPackageInfo> GetNugetDependencies()
    {
        yield return NugetPackages.MicrosoftAzureWebJobsExtensionsStorageQueues(_template.OutputTarget);
    }
}