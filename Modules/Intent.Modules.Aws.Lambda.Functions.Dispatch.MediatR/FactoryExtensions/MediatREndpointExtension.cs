using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.Aws.Lambda.Functions.Api;
using Intent.Modules.Aws.Lambda.Functions.Dispatch.MediatR.Templates.Endpoints;
using Intent.Modules.Aws.Lambda.Functions.Templates.LambdaFunctionClass;
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

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Dispatch.MediatR.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MediatREndpointExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Aws.Lambda.Functions.Dispatch.MediatR.MediatREndpointExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<LambdaFunctionClassTemplate>(LambdaFunctionClassTemplate.TemplateId);
            foreach (var containerTemplate in templates)
            {
                if (containerTemplate.Model is not CqrsLambdaFunctionContainerModel)
                {
                    continue;
                }

                containerTemplate.AddTypeSource(CommandModelsTemplate.TemplateId);
                containerTemplate.AddTypeSource(QueryModelsTemplate.TemplateId);
                containerTemplate.AddTypeSource(ClassTypeSource.Create(application, "Application.Contract.Dto")
                    .WithCollectionFormatter(CSharpCollectionFormatter.CreateList()));
                containerTemplate.CSharpFile.OnBuild(file =>
                {
                    file.AddUsing("MediatR");
                    file.AddUsing("Microsoft.Extensions.DependencyInjection");

                    var @class = file.Classes.First();
                    var ctor = @class.Constructors.First();
                    ctor.AddParameter(containerTemplate.UseType("MediatR.ISender"), "mediator",
                        p => p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()));

                    foreach (var actionMethod in @class.Methods)
                    {
                        var operationModel = (ILambdaFunctionModel)actionMethod.RepresentedModel;
                        
                        actionMethod.InsertStatement(0, $"""
                                                         // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
                                                         var cancellationToken = {containerTemplate.UseType("System.Threading.CancellationToken")}.None;
                                                         """);
                        
                        AddCqrsParameterToFieldAssignments(application, actionMethod, operationModel);
                        actionMethod.AddStatements(GetValidations(containerTemplate, operationModel));
                        actionMethod.AddStatement(GetDispatchViaMediatorStatement(containerTemplate, operationModel), s => s.SeparatedFromPrevious());
                        
                        var returnStatement = actionMethod.Statements.LastOrDefault(x => x.ToString()!.Trim().StartsWith("return "));
                        if (returnStatement != null)
                        {
                            returnStatement.Remove();
                            actionMethod.AddStatement(containerTemplate.GetReturnStatement(operationModel));
                        }
                    }
                }, 10);
            }
        }

        private static void AddCqrsParameterToFieldAssignments(
            IApplication application,
            CSharpClassMethod actionMethod,
            ILambdaFunctionModel operationModel)
        {
            var payloadParameter = GetPayloadParameter(operationModel);
            if (payloadParameter == null)
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
            IEndpointParameterModel payloadParameter)
        {
            var statements = new List<CSharpStatement>();
            var commandClass = cqrsModelTemplate.CSharpFile.Classes.First();

            foreach (var methodParameter in actionMethod.Parameters)
            {
                var paramModelId = ((IEndpointParameterModel)methodParameter.RepresentedModel).Id;

                var commandProp = commandClass.Properties.FirstOrDefault(prop =>
                    (prop.RepresentedModel as IEndpointParameterModel)?.Id == paramModelId);
                if (commandProp is null)
                {
                    continue;
                }

                statements.Add(new CSharpIfStatement($"{payloadParameter.Name}.{commandProp.Name} == {GetComparisonValue(commandProp.Type?.ToTypeName() ?? string.Empty)}")
                    .AddStatement($"{payloadParameter.Name}.{commandProp.Name} = {methodParameter.Name};"));
            }

            return statements;
        }

        private static string GetComparisonValue(string type) => type switch
        {
            "Guid" => "Guid.Empty",
            _ => "default"
        };


        private IEnumerable<CSharpStatement> GetValidations(LambdaFunctionClassTemplate containerTemplate, ILambdaFunctionModel operationModel)
        {
            var validations = new List<CSharpStatement>();
            var payloadParameter = GetPayloadParameter(operationModel);
            if (payloadParameter == null)
            {
                return validations;
            }

            foreach (var mappedParameter in GetMappedParameters(operationModel))
            {
                validations.Add(new CSharpIfStatement(
                        $"{(mappedParameter.TypeReference.HasGuidType() ? $"{mappedParameter.Name.ToParameterName()}Guid" : mappedParameter.Name.ToParameterName())} != {payloadParameter.Name}.{mappedParameter.MappedPayloadProperty.Name.ToPascalCase()}")
                    .AddStatement($"return {containerTemplate.UseType("Amazon.Lambda.Annotations.APIGateway.HttpResults")}.BadRequest();"));
            }

            return validations;
        }

        private CSharpStatement GetDispatchViaMediatorStatement(LambdaFunctionClassTemplate template, ILambdaFunctionModel operationModel)
        {
            var payload = GetPayloadParameter(operationModel)?.Name
                          ?? GetMappedPayload(template, operationModel);

            return operationModel.ReturnType != null
                ? $"var result = await _mediator.Send({payload}, cancellationToken);"
                : $"await _mediator.Send({payload}, cancellationToken);";
        }

        private static IEndpointParameterModel? GetPayloadParameter(ILambdaFunctionModel operationModel)
        {
            return operationModel.Parameters.SingleOrDefault(x =>
                x.TypeReference.Element.SpecializationTypeId == CommandModel.SpecializationTypeId ||
                x.TypeReference.Element.SpecializationTypeId == QueryModel.SpecializationTypeId);
        }

        private string GetMappedPayload(LambdaFunctionClassTemplate template, ILambdaFunctionModel operationModel)
        {
            var requestType = operationModel.InternalElement.IsCommandModel() || operationModel.InternalElement.IsQueryModel()
                ? operationModel.InternalElement.AsTypeReference()
                : operationModel.InternalElement.MappedElement;

            if (GetMappedParameters(operationModel).Any())
            {
                return
                    $"new {template.GetTypeName(requestType)} ( {string.Join(", ", GetMappedParameters(operationModel).Select(x => x.MappedPayloadProperty.TypeReference.HasGuidType() ? $"{x.MappedPayloadProperty.Name.ToParameterName()}Guid" : x.MappedPayloadProperty.Name.ToParameterName() + " : " + x.Name))} )";
            }

            return $"new {template.GetTypeName(requestType)}()";
        }

        private IList<IEndpointParameterModel> GetMappedParameters(ILambdaFunctionModel operationModel)
        {
            return operationModel.Parameters.Where(x => x.MappedPayloadProperty != null).ToList();
        }
    }
}