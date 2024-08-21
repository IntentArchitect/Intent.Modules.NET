using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.AspNetCore.Controllers.Dispatch.MediatR.ImplicitControllers;
using Intent.Modules.AspNetCore.Controllers.Templates;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Build.Evaluation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Dispatch.MediatR.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class MediatRControllerInstaller : FactoryExtensionBase
{
    public override string Id => "Intent.AspNetCore.Controllers.Dispatch.MediatR.MediatRControllerInstaller";

    [IntentManaged(Mode.Ignore)] public override int Order => 0;

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        var controllerTemplates = application.FindTemplateInstances<ControllerTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Distribution.WebApi.Controller));
        foreach (var controllerTemplate in controllerTemplates)
        {
            if (controllerTemplate.Model is not CqrsControllerModel)
            {
                continue;
            }

            controllerTemplate.AddTypeSource(CommandModelsTemplate.TemplateId);
            controllerTemplate.AddTypeSource(QueryModelsTemplate.TemplateId);
            controllerTemplate.AddTypeSource(ClassTypeSource.Create(application, "Application.Contract.Dto")
                .WithCollectionFormatter(CSharpCollectionFormatter.CreateList()));
            controllerTemplate.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("MediatR");
                file.AddUsing("Microsoft.AspNetCore.Mvc");
                file.AddUsing("Microsoft.Extensions.DependencyInjection");

                var controllerClass = file.Classes.First();
                var ctor = controllerClass.Constructors.First();
                ctor.AddParameter(controllerTemplate.UseType("MediatR.ISender"), "mediator",
                    p => p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()));

                foreach (var actionMethod in controllerClass.Methods)
                {
                    if (!actionMethod.TryGetMetadata<IControllerOperationModel>("model", out var operationModel))
                    {
                        continue;

                    }
                    if (FileTransferHelper.IsFileUploadOperation(operationModel))
                    {
                        CreateUploadCommand(application, controllerTemplate, actionMethod, operationModel);
                    }

                    AddCqrsParameterToFieldAssignments(application, actionMethod, operationModel);
                    actionMethod.AddStatements(GetValidations(operationModel));
                    actionMethod.AddStatement(GetDispatchViaMediatorStatement(controllerTemplate, operationModel), s => s.SeparatedFromPrevious());
                    actionMethod.AddStatement(controllerTemplate.GetReturnStatement(operationModel));
                }
            }, 10);
        }
    }

    private void CreateUploadCommand(IApplication application, ControllerTemplate template, CSharpClassMethod method, IControllerOperationModel operation)
    {
        var fileInfo = FileTransferHelper.GetUploadTypeInfo(operation);

        FileTransferHelper.AddControllerStreamLogic(template, method, operation);

        var parameters = new StringBuilder();
        var commandType = operation.Parameters.First(p => p.Source == HttpInputSource.FromBody);
        foreach (var parameter in operation.Parameters.Where(p => p.MappedPayloadProperty != null))
        {
            if (fileInfo.HasFilename() && string.Equals(fileInfo.FileNameField, parameter.MappedPayloadProperty.Name))
            {
                continue;
            }

            parameters.Append(", ");
            parameters.Append(((IElement)parameter.MappedPayloadProperty).Name.ToParameterName());
            parameters.Append(": ");
            parameters.Append(parameter.Name.ToParameterName());

        }
        method.AddStatement($"var command = new {template.GetTypeName(commandType)}({fileInfo.StreamField.ToParameterName()}: stream{(fileInfo.HasFilename() ? $", {fileInfo.FileNameField.ToParameterName()}: {fileInfo.FileNameField.ToParameterName()}" : "")} {parameters});");

    }

    private static void AddCqrsParameterToFieldAssignments(
        IApplication application,
        CSharpClassMethod actionMethod,
        IControllerOperationModel operationModel)
    {
        var payloadParameter = GetPayloadParameter(operationModel);
        if (payloadParameter == null)
        {
            return;
        }

        if (FileTransferHelper.IsFileUploadOperation(operationModel))
        {
            return;
        }

        var commandModelTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(CommandModelsTemplate.TemplateId, operationModel.InternalElement.Id);
        if (commandModelTemplate is not null)
        {
            commandModelTemplate.CSharpFile.OnBuild(file =>
            {
                var statements = GetGenericParameterToFieldStatements(actionMethod, commandModelTemplate, payloadParameter);
                var index = -1;
                foreach (var statement in statements)
                {
                    index++;
                    actionMethod.InsertStatement(index, statement);
                }
            }, 10);
        }

        var queryModelTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(QueryModelsTemplate.TemplateId, operationModel.InternalElement.Id);
        if (queryModelTemplate is not null)
        {
            queryModelTemplate.CSharpFile.OnBuild(file =>
            {
                var statements = GetGenericParameterToFieldStatements(actionMethod, queryModelTemplate, payloadParameter);
                var index = -1;
                foreach (var statement in statements)
                {
                    index++;
                    actionMethod.InsertStatement(index, statement);
                }
            }, 10);
        }
    }

    private static IReadOnlyCollection<CSharpStatement> GetGenericParameterToFieldStatements(
        CSharpClassMethod actionMethod,
        ICSharpFileBuilderTemplate cqrsModelTemplate,
        IControllerParameterModel payloadParameter)
    {
        var statements = new List<CSharpStatement>();
        var commandClass = cqrsModelTemplate.CSharpFile.Classes.First();

        foreach (var methodParameter in actionMethod.Parameters)
        {
            if (!methodParameter.TryGetMetadata<string>("modelId", out var paramModelId))
            {
                continue;
            }

            var commandProp = commandClass.Properties.FirstOrDefault(prop =>
                prop.TryGetMetadata<DTOFieldModel>("model", out var propModel) &&
                propModel?.Id == paramModelId);
            if (commandProp is null)
            {
                continue;
            }

            statements.Add(new CSharpIfStatement($"{payloadParameter.Name}.{commandProp.Name} == default")
                .AddStatement($"{payloadParameter.Name}.{commandProp.Name} = {methodParameter.Name};"));
        }

        return statements;
    }

    private IEnumerable<CSharpStatement> GetValidations(IControllerOperationModel operationModel)
    {
        var validations = new List<CSharpStatement>();
        var payloadParameter = GetPayloadParameter(operationModel);
        if (payloadParameter == null)
        {
            return validations;
        }

        if (FileTransferHelper.IsFileUploadOperation(operationModel))
        {
            return validations;
        }

        foreach (var mappedParameter in GetMappedParameters(operationModel))
        {
            validations.Add(new CSharpIfStatement($"{mappedParameter.Name} != {payloadParameter.Name}.{mappedParameter.MappedPayloadProperty.Name.ToPascalCase()}")
                .AddStatement("return BadRequest();"));
        }

        return validations;
    }

    private CSharpStatement GetDispatchViaMediatorStatement(ControllerTemplate template, IControllerOperationModel operationModel)
    {
        var payload = GetPayloadParameter(operationModel)?.Name
                      ?? GetMappedPayload(template, operationModel);

        return operationModel.ReturnType != null
            ? $"var result = await _mediator.Send({payload}, cancellationToken);"
            : $"await _mediator.Send({payload}, cancellationToken);";
    }

    private static IControllerParameterModel GetPayloadParameter(IControllerOperationModel operation)
    {
        return operation.Parameters.SingleOrDefault(x =>
            x.TypeReference.Element.SpecializationTypeId == CommandModel.SpecializationTypeId ||
            x.TypeReference.Element.SpecializationTypeId == QueryModel.SpecializationTypeId);
    }

    private string GetMappedPayload(ControllerTemplate controllerTemplate, IControllerOperationModel operation)
    {
        var requestType = operation.InternalElement.IsCommandModel() || operation.InternalElement.IsQueryModel()
            ? operation.InternalElement.AsTypeReference()
            : operation.InternalElement.MappedElement;

        if (GetMappedParameters(operation).Any())
        {
            return
                $"new {controllerTemplate.GetTypeName(requestType)} ( {string.Join(", ", GetMappedParameters(operation).Select(x => x.MappedPayloadProperty.Name.ToParameterName() + " : " + x.Name))} )";
        }

        return $"new {controllerTemplate.GetTypeName(requestType)}()";
    }

    private IList<IControllerParameterModel> GetMappedParameters(IControllerOperationModel operationModel)
    {
        return operationModel.Parameters.Where(x => x.MappedPayloadProperty != null).ToList();
    }
}