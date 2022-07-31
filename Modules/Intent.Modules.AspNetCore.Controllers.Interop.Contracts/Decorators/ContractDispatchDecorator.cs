using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Interop.Contracts.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ContractDispatchDecorator : ControllerDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.Controllers.Interop.Contracts.ContractDispatchDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly ControllerTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public ContractDispatchDecorator(ControllerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _template.AddTypeSource(DtoModelTemplate.TemplateId, "List<{0}>");
        }

        public override string EnterClass()
        {
            return $@"
        private readonly {_template.GetTypeName(ServiceContractTemplate.TemplateId, _template.Model)} _appService;";
        }

        public override IEnumerable<string> ConstructorParameters()
        {
            return new[] { $"{_template.GetTypeName(ServiceContractTemplate.TemplateId, _template.Model)} appService" };
        }

        public override string ConstructorImplementation()
        {
            return $@"
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));";
        }

        public override string EnterOperationBody(OperationModel operationModel)
        {
            if (operationModel.ReturnType != null)
            {
                return $@"
            var result = default({_template.GetTypeName(operationModel)});";
            }

            return null;
        }

        public override string MidOperationBody(OperationModel operationModel)
        {
            if (operationModel.ReturnType != null)
            {
                return $@"
            result = await _appService.{operationModel.Name.ToPascalCase()}({_template.GetArguments(operationModel.Parameters)});
";
            }
            return $@"
            await _appService.{operationModel.Name.ToPascalCase()}({_template.GetArguments(operationModel.Parameters)});";
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
    }
}