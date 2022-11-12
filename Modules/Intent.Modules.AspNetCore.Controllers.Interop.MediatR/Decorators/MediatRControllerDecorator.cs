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
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Interop.MediatR.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class MediatRControllerDecorator : ControllerDecorator
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
            _template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("MediatR");
                file.AddUsing("Microsoft.AspNetCore.Mvc");
                file.AddUsing("Microsoft.Extensions.DependencyInjection");

                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                ctor.AddParameter(_template.UseType("MediatR.ISender"), "mediator", p =>
                {
                    p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                });

                foreach (var method in @class.Methods)
                {
                    if (method.TryGetMetadata<OperationModel>("model", out var model) &&
                        model.HasMapToCommandMapping() || model.HasMapToQueryMapping())
                    {
                        method.AddStatements(GetValidations(model));
                        method.AddStatement(GetDispatchViaMediatorStatement(model), s => s.SeparatedFromPrevious());
                        method.AddStatement(GetReturnStatement(model));
                    }
                }
            });
        }

        //public override string EnterClass()
        //{
        //    return $@"
        //private readonly ISender _mediator;";
        //}

        //public override IEnumerable<string> ConstructorParameters()
        //{
        //    return new[] { $"ISender mediator" };
        //}

        //public override string ConstructorImplementation()
        //{
        //    return $@"
        //    _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));";
        //}

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

        private CSharpStatement GetDispatchViaMediatorStatement(OperationModel operationModel)
        {
            var payload = GetPayloadParameter(operationModel)?.Name
                ?? (operationModel.InternalElement.IsMapped ? GetMappedPayload(operationModel) : "UNKNOWN");

            return operationModel.ReturnType != null
                ? $"var result = await _mediator.Send({payload}, cancellationToken);"
                : $@"await _mediator.Send({payload}, cancellationToken);";
        }

        private CSharpStatement GetReturnStatement(OperationModel operationModel)
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
                return $"new {_template.GetTypeName(mappedElement)} {{ {string.Join(", ", GetMappedParameters(operationModel).Select(x => x.InternalElement.MappedElement.Element.Name.ToPascalCase() + " = " + x.Name))} }}";
            }
            return $"new {_template.GetTypeName(mappedElement)}()";
        }

        public IList<ParameterModel> GetMappedParameters(OperationModel operationModel)
        {
            return operationModel.Parameters.Where(x => x.InternalElement.IsMapped).ToList();
        }
    }
}