using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Application.MediatR.Api;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Dispatch.MediatR.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AzureFunctionsMediatRDispatchInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.AzureFunctions.Dispatch.MediatR.AzureFunctionsMediatRDispatchInstaller";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<AzureFunctionClassTemplate>(TemplateDependency.OnTemplate(AzureFunctionClassTemplate.TemplateId));
            foreach (var template in templates)
            {
                var mappedCommand = template.Model.InternalElement.AsCommandModel() ?? template.Model.Mapping?.Element?.AsCommandModel();
                var mappedQuery = template.Model.InternalElement.AsQueryModel() ?? template.Model.Mapping?.Element?.AsQueryModel();

                if (mappedCommand != null || mappedQuery != null)
                {
                    template.AddTypeSource(CommandModelsTemplate.TemplateId);
                    template.AddTypeSource(QueryModelsTemplate.TemplateId);

                    template.CSharpFile.OnBuild(file =>
                    {
                        var @class = file.Classes.First();
                        var ctor = @class.Constructors.First();
                        ctor.AddParameter(template.UseType("MediatR.IMediator"), "mediator",
                            p =>
                            {
                                p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                            });


                        var runMethod = @class.FindMethod("Run");
                        ((IHasCSharpStatements)runMethod.FindStatement<CSharpTryBlock>(x => true) ?? runMethod)?
                            .AddStatements(GetValidations(template.Model))
                            .AddStatement(GetDispatchViaMediatorStatement(template, template.Model))
                            .AddStatements(GetReturnStatement(template, template.Model));
                    });
                }
            }
        }


        private IEnumerable<CSharpStatement> GetValidations(IAzureFunctionModel model)
        {
            var validations = new List<string>();
            var payloadParameter = GetPayloadParameter(model);
            if (payloadParameter != null)
            {
                foreach (var mappedParameter in GetMappedParameters(model))
                {
                    validations.Add($@"
            if ({mappedParameter.Name} != {payloadParameter.Name}.{mappedParameter.MappedPath.ToPascalCase()})
            {{
                return new BadRequestObjectResult(new {{ Message = ""Supplied '{mappedParameter.Name}' does not match '{mappedParameter.MappedPath.ToPascalCase()}' from body."" }});
            }}
            ");
                }
            }

            return validations.Select(x => (CSharpStatement)x).ToList();
        }

        private CSharpStatement GetDispatchViaMediatorStatement(ICSharpFileBuilderTemplate template, IAzureFunctionModel model)
        {
            //
            var payload = GetPayloadParameter(model)?.Name
                ?? GetMappedPayload(template, model);

            return model.ReturnType != null
                ? new CSharpStatement($"var result = await _mediator.Send({payload}, cancellationToken);")
                : new CSharpStatement($@"await _mediator.Send({payload}, cancellationToken);");
        }

        private IEnumerable<CSharpStatement> GetReturnStatement(ICSharpFileBuilderTemplate template, IAzureFunctionModel operationModel)
        {
            var endpoint = HttpEndpointModelFactory.GetEndpoint(operationModel.InternalElement, "");
            switch (endpoint?.Verb)
            {
                case HttpVerb.Get:
                    if (operationModel.ReturnType == null)
                    {
                        yield return "return new NoContentResult();";
                        break;
                    }

                    if (operationModel.ReturnType.IsCollection)
                    {
                        yield return "return new OkObjectResult(result);";
                        break;
                    }

                    yield return @"return result != null ? new OkObjectResult(result) : new NotFoundResult();";
                    break;
                case HttpVerb.Post:
                    var getByIdFunction = (operationModel.InternalElement.ParentElement?.ChildElements ?? operationModel.InternalElement.Package.ChildElements)
                            .Where(x => x.IsAzureFunctionModel())
                            .Select(x => x.AsAzureFunctionModel())
                            .FirstOrDefault(x => x.GetAzureFunction().Method().IsGET() &&
                                x.ReturnType != null &&
                                !x.ReturnType.IsCollection &&
                                x.Parameters.Count() == 1 &&
                                x.Parameters.FirstOrDefault()?.Name == "id");
                    if (getByIdFunction != null && new[] { "guid", "long", "int" }.Contains(operationModel.ReturnType?.Element.Name))
                    {
                        yield return $@"return new CreatedResult(new Uri(""{getByIdFunction.GetAzureFunction().Route()}"", UriKind.Relative), new {{ id = result }});";
                        break;
                    }
                    yield return operationModel.ReturnType == null ? @"return new CreatedResult(string.Empty, null);" : @"return new CreatedResult(string.Empty, result);";
                    break;
                case HttpVerb.Put:
                case HttpVerb.Patch:
                    yield return operationModel.ReturnType == null ? @"return new NoContentResult();" : @"return new OkObjectResult(result);";
                    break;
                case HttpVerb.Delete:
                    yield return operationModel.ReturnType == null ? @"return new OkResult();" : @"return new OkObjectResult(result);";
                    break;
                case null:
                    if (operationModel.ReturnType != null)
                    {
                        yield return $"return result;";
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IAzureFunctionParameterModel GetPayloadParameter(IAzureFunctionModel operation)
        {
            return operation.Parameters.SingleOrDefault(x =>
                x.TypeReference.Element.SpecializationTypeId == CommandModel.SpecializationTypeId ||
                x.TypeReference.Element.SpecializationTypeId == QueryModel.SpecializationTypeId);
        }

        private string GetMappedPayload(ICSharpFileBuilderTemplate template, IAzureFunctionModel operation)
        {
            var requestType = operation.InternalElement.IsCommandModel() || operation.InternalElement.IsQueryModel()
                ? operation.InternalElement.AsTypeReference()
                : operation.InternalElement.MappedElement;

            if (GetMappedParameters(operation).Any())
            {
                return $"new {template.GetTypeName(requestType)} ( {string.Join(", ", GetMappedParameters(operation).Select(x => x.MappedPath.ToParameterName() + " : " + x.Name.ToParameterName()))} )";
            }

            return $"new {template.GetTypeName(requestType)}()";
        }

        private IList<IAzureFunctionParameterModel> GetMappedParameters(IAzureFunctionModel operationModel)
        {
            return operationModel.Parameters.Where(x => x.IsMapped).ToList();
        }
    }
}