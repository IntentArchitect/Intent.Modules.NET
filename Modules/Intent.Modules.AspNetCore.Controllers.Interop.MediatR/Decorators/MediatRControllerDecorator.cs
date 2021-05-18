using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
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
        private readonly ControllerTemplate _template;

        public MediatRControllerDecorator(ControllerTemplate template)
        {
            _template = template;
            _template.AddTypeSource(CommandModelsTemplate.TemplateId);
            _template.AddTypeSource(QueryModelsTemplate.TemplateId);
            _template.AddTypeSource(DtoModelTemplate.TemplateId, "List<{0}>");
        }

        public override string EnterClass()
        {
            return @"
        private ISender _mediator;

        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetService<ISender>();
";
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
            if ({mappedParameter.Name} != {payloadParameter.Name}.{mappedParameter.InternalElement.MappedElement.Element.Name})
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
                ? $"var result = await Mediator.Send({payload}, cancellationToken);"
                : $@"await Mediator.Send({payload}, cancellationToken);";
        }

        public override string ExitOperationBody(OperationModel operationModel)
        {
            return operationModel.ReturnType == null
                ? $@"
            return NoContent();"
                : $@"
            return result;";
        }

        private ParameterModel GetPayloadParameter(OperationModel operationModel)
        {
            return operationModel.Parameters.SingleOrDefault(x =>
                x.Type.Element.SpecializationTypeId == CommandModel.SpecializationTypeId ||
                x.Type.Element.SpecializationTypeId == QueryModel.SpecializationTypeId);
        }

        public IList<ParameterModel> GetMappedParameters(OperationModel operationModel)
        {
            return operationModel.Parameters.Where(x => x.InternalElement.IsMapped).ToList();
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