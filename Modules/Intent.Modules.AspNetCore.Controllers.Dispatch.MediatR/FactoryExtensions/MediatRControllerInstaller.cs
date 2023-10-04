using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
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
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Dispatch.MediatR.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MediatRControllerInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Controllers.Dispatch.MediatR.MediatRControllerInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<ControllerTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Distribution.WebApi.Controller));
            foreach (var template in templates)
            {
                if (template.Model is not CqrsControllerModel)
                {
                    continue;
                }

                template.AddTypeSource(CommandModelsTemplate.TemplateId);
                template.AddTypeSource(QueryModelsTemplate.TemplateId);
                template.AddTypeSource(ClassTypeSource.Create(application, "Application.Contract.Dto").WithCollectionFormatter(CSharpCollectionFormatter.CreateList()));
                template.CSharpFile.OnBuild(file =>
                {
                    file.AddUsing("MediatR");
                    file.AddUsing("Microsoft.AspNetCore.Mvc");
                    file.AddUsing("Microsoft.Extensions.DependencyInjection");

                    var @class = file.Classes.First();
                    var ctor = @class.Constructors.First();
                    ctor.AddParameter(template.UseType("MediatR.ISender"), "mediator", p =>
                    {
                        p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                    });

                    foreach (var method in @class.Methods)
                    {
                        if (method.TryGetMetadata<IControllerOperationModel>("model", out var model))
                        {
                            method.AddStatements(GetValidations(model));
                            method.AddStatement(GetDispatchViaMediatorStatement(template, model), s => s.SeparatedFromPrevious());
                            method.AddStatement(template.GetReturnStatement(model));
                        }
                    }
                });
            }
        }

        private IEnumerable<string> GetValidations(IControllerOperationModel operationModel)
        {
            var validations = new List<string>();
            var payloadParameter = GetPayloadParameter(operationModel);
            if (payloadParameter != null)
            {
                foreach (var mappedParameter in GetMappedParameters(operationModel))
                {
                    validations.Add($@"
            if ({mappedParameter.Name} != {payloadParameter.Name}.{mappedParameter.MappedPayloadProperty.Name.ToPascalCase()})
            {{
                return BadRequest();
            }}
            ");
                }
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

        private string GetMappedPayload(ControllerTemplate template, IControllerOperationModel operation)
        {
            var requestType = operation.InternalElement.IsCommandModel() || operation.InternalElement.IsQueryModel()
                ? operation.InternalElement.AsTypeReference()
                : operation.InternalElement.MappedElement;

            if (GetMappedParameters(operation).Any())
            {
                //return $"new {template.GetTypeName(requestType)} {{ {string.Join(", ", GetMappedParameters(operation).Select(x => x.MappedPayloadProperty.Name.ToPascalCase() + " = " + x.Name))} }}";
                return $"new {template.GetTypeName(requestType)} ( {string.Join(", ", GetMappedParameters(operation).Select(x => x.MappedPayloadProperty.Name.ToParameterName() + " : " + x.Name))} )";
            }

            return $"new {template.GetTypeName(requestType)}()";
        }

        private IList<IControllerParameterModel> GetMappedParameters(IControllerOperationModel operationModel)
        {
            return operationModel.Parameters.Where(x => x.MappedPayloadProperty != null).ToList();
        }
    }
}