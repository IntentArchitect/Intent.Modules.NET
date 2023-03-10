using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
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
            var templates = application.FindTemplateInstances<ControllerTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Application.Services.Controllers));
            foreach (var template in templates)
            {
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
                        if (method.TryGetMetadata<OperationModel>("model", out var model) &&
                            model.HasMapToCommandMapping() || model.HasMapToQueryMapping())
                        {
                            method.AddStatements(GetValidations(model));
                            method.AddStatement(GetDispatchViaMediatorStatement(template, model), s => s.SeparatedFromPrevious());
                            method.AddStatement(GetReturnStatement(template, model));
                        }
                    }
                });
            }
        }
        
        private IEnumerable<string> GetValidations(OperationModel operationModel)
        {
            var validations = new List<string>();
            var payloadParameter = GetPayloadParameter(operationModel);
            if (payloadParameter != null && operationModel.InternalElement.IsMapped)
            {
                foreach (var mappedParameter in GetMappedParameters(operationModel))
                {
                    validations.Add($@"
            if ({mappedParameter.Name} != {payloadParameter.Name}.{mappedParameter.InternalElement.MappedElement.Element.Name.ToPascalCase()})
            {{
                return BadRequest();
            }}
            ");
                }
            }

            return validations;
        }

        private CSharpStatement GetDispatchViaMediatorStatement(ControllerTemplate template, OperationModel operationModel)
        {
            var payload = GetPayloadParameter(operationModel)?.Name
                ?? (operationModel.InternalElement.IsMapped ? GetMappedPayload(template, operationModel) : "UNKNOWN");

            return operationModel.ReturnType != null
                ? $"var result = await _mediator.Send({payload}, cancellationToken);"
                : $@"await _mediator.Send({payload}, cancellationToken);";
        }

        private CSharpStatement GetReturnStatement(ControllerTemplate template, OperationModel operationModel)
        {
            switch (template.GetHttpVerb(operationModel))
            {
                case ControllerTemplate.HttpVerb.Get:
                    if (operationModel.ReturnType == null)
                    {
                        return "return NoContent();";
                    }

                    if (operationModel.ReturnType.IsCollection)
                    {
                        return "return Ok(result);";
                    }

                    return @"return result != null ? Ok(result) : NotFound();";
                case ControllerTemplate.HttpVerb.Post:
                    var getByIdOperation = template.Model.Operations.FirstOrDefault(x => (x.Name == "Get" || x.Name == $"Get{operationModel.Name.Replace("Create", "")}") && x.Parameters.FirstOrDefault()?.Name == "id");
                    if (getByIdOperation != null && new[] { "guid", "long", "int" }.Contains(operationModel.ReturnType?.Element.Name))
                    {
                        return @"return CreatedAtAction(nameof(Get), new { id = result }, new { Id = result });";
                    }
                    return operationModel.ReturnType == null ? @"return Created(string.Empty, null);" : @"return Created(string.Empty, result);";
                case ControllerTemplate.HttpVerb.Put:
                case ControllerTemplate.HttpVerb.Patch:
                    return operationModel.ReturnType == null ? @"return NoContent();" : @"return Ok(result);";
                case ControllerTemplate.HttpVerb.Delete:
                    return operationModel.ReturnType == null ? @"return Ok();" : @"return Ok(result);";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static ParameterModel GetPayloadParameter(OperationModel operationModel)
        {
            return operationModel.Parameters.SingleOrDefault(x =>
                x.Type.Element.SpecializationTypeId == CommandModel.SpecializationTypeId ||
                x.Type.Element.SpecializationTypeId == QueryModel.SpecializationTypeId);
        }

        private string GetMappedPayload(ControllerTemplate template, OperationModel operationModel)
        {
            var mappedElement = operationModel.InternalElement.MappedElement;
            if (GetMappedParameters(operationModel).Any())
            {
                return $"new {template.GetTypeName(mappedElement)} {{ {string.Join(", ", GetMappedParameters(operationModel).Select(x => x.InternalElement.MappedElement.Element.Name.ToPascalCase() + " = " + x.Name))} }}";
            }

            return $"new {template.GetTypeName(mappedElement)}()";
        }

        private IList<ParameterModel> GetMappedParameters(OperationModel operationModel)
        {
            return operationModel.Parameters.Where(x => x.InternalElement.IsMapped).ToList();
        }
    }
}