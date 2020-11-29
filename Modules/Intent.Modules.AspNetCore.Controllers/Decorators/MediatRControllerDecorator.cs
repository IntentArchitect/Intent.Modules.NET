using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Application.MediatR.Templates.DtoModel;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common;
using FolderExtensionModel = Intent.Modelers.Services.Api.FolderExtensionModel;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.AspNetCore.Controllers.Decorators
{
    public class MediatRControllerDecorator : ControllerDecorator
    {
        private readonly ControllerTemplate _template;
        public const string DecoratorId = "Intent.AspNetCore.Controllers.MediatRDecorator";

        public MediatRControllerDecorator(ControllerTemplate template)
        {
            _template = template;
            _template.AddTypeSource(CommandModelsTemplate.TemplateId);
            _template.AddTypeSource(QueryModelsTemplate.TemplateId);
            _template.AddTypeSource(DtoModelTemplate.TemplateId);
        }

        public override string OnEnterOperationBody(OperationModel operationModel)
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

        public override string OnExitOperationBody(OperationModel operationModel)
        {
            var payload = GetPayloadParameter(operationModel)?.Name
                ?? (operationModel.InternalElement.IsMapped ? GetMappedPayload(operationModel) : "UNKNOWN");
            if (operationModel.ReturnType != null)
            {
                return $"return await Mediator.Send({payload});";
            }

            return $@"await Mediator.Send({payload});

            return NoContent();";
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
                return $"new {_template.GetTypeName(mappedElement)} {{ {string.Join(", ", GetMappedParameters(operationModel).Select(x => x.InternalElement.MappedElement.Element.Name + " = " + x.Name))}}}";
            }
            return $"new {_template.GetTypeName(mappedElement)}()";
        }
    }
}