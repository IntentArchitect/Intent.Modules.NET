using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Interop.MediatR.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class MediatRControllerDecorator : ControllerDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.Controllers.Interop.MediatR.MediatRControllerDecorator";
        [IntentManaged(Mode.Fully)]
        private readonly ControllerTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public MediatRControllerDecorator(ControllerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _template.AddTypeSource(CommandModelsTemplate.TemplateId);
            _template.AddTypeSource(QueryModelsTemplate.TemplateId);
            _template.AddTypeSource("Application.Contract.Dto", "List<{0}>");
        }

        public override string EnterClass()
        {
            return $@"
        private readonly ISender _mediator;";
        }

        public override IEnumerable<string> ConstructorParameters()
        {
            return new[] { $"ISender mediator" };
        }

        public override string ConstructorImplementation()
        {
            return $@"
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));";
        }

        public override string EnterOperationBody(OperationModel operationModel)
        {
            var validations = new StringBuilder();
            var payloadParameter = GetPayloadParameter(operationModel);
            if (payloadParameter != null && operationModel.InternalElement.IsMapped)
            {
                foreach (var mappedParameter in GetMappedParameters(operationModel))
                {
                    validations.Append($@"
            if ({mappedParameter.Name} != {payloadParameter.Name}.{mappedParameter.InternalElement.MappedElement.Element.Name.ToPascalCase()})
            {{
                return BadRequest();
            }}
            ");
                }
            }

            return validations.ToString();
        }

        public override string MidOperationBody(OperationModel operationModel)
        {
            var payload = GetPayloadParameter(operationModel)?.Name
                ?? (operationModel.InternalElement.IsMapped ? GetMappedPayload(operationModel) : "UNKNOWN");

            return operationModel.ReturnType != null
                ? $"var result = await _mediator.Send({payload}, cancellationToken);"
                : $@"await _mediator.Send({payload}, cancellationToken);";
        }

        public override string ExitOperationBody(OperationModel operationModel)
        {
            switch (_template.GetHttpVerb(operationModel))
            {
                case ControllerTemplate.HttpVerb.GET:
                    return operationModel.ReturnType == null ? $@"return NoContent();" : $@"return Ok(result);";
                case ControllerTemplate.HttpVerb.POST:
                    var getByIdOperation = _template.Model.Operations.FirstOrDefault(x => (x.Name == "Get" || x.Name == $"Get{operationModel.Name.Replace("Create", "")}") && x.Parameters.FirstOrDefault()?.Name == "id");
                    if (getByIdOperation != null && new[] { "guid", "long", "int" }.Contains(operationModel.ReturnType?.Element.Name))
                    {
                        return $@"return CreatedAtAction(nameof(Get), new {{ id = result }}, new {{ Id = result }});";
                    }
                    return operationModel.ReturnType == null ? $@"return Created(string.Empty, null);" : $@"return Created(string.Empty, result);";
                case ControllerTemplate.HttpVerb.PUT:
                case ControllerTemplate.HttpVerb.PATCH:
                    return operationModel.ReturnType == null ? $@"return NoContent();" : $@"return Ok(result);";
                case ControllerTemplate.HttpVerb.DELETE:
                    return operationModel.ReturnType == null ? $@"return Ok();" : $@"return Ok(result);";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private ParameterModel GetPayloadParameter(OperationModel operationModel)
        {
            return operationModel.Parameters.SingleOrDefault(x =>
                x.Type.Element.SpecializationTypeId == CommandModel.SpecializationTypeId ||
                x.Type.Element.SpecializationTypeId == QueryModel.SpecializationTypeId);
        }

        private string GetMappedPayload(OperationModel operationModel)
        {
            var mappedElement = operationModel.InternalElement.MappedElement;
            if (GetMappedParameters(operationModel).Any())
            {
                return $"new {_template.GetTypeName(mappedElement)} {{ {string.Join(", ", GetMappedParameters(operationModel).Select(x => x.InternalElement.MappedElement.Element.Name.ToPascalCase() + " = " + x.Name))}}}";
            }
            return $"new {_template.GetTypeName(mappedElement)}()";
        }

        public IList<ParameterModel> GetMappedParameters(OperationModel operationModel)
        {
            return operationModel.Parameters.Where(x => x.InternalElement.IsMapped).ToList();
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "MediatR",
                "Microsoft.AspNetCore.Mvc",
                "Microsoft.Extensions.DependencyInjection"
            };
        }
    }
}