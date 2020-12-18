using System;
using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Dispatch.Contracts.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ContractDispatchDecorator : ControllerDecorator
    {
        public const string DecoratorId = "AspNetCore.Controllers.Dispatch.Contracts.ContractDispatchDecorator";

        private readonly ControllerTemplate _template;

        public ContractDispatchDecorator(ControllerTemplate template)
        {
            _template = template;
        }

        public override string EnterClass()
        {
            return $@"
        private readonly {_template.GetTypeName(ServiceContractTemplate.IDENTIFIER, _template.Model)} _appService;";
        }

        public override IEnumerable<string> ConstructorParameters()
        {
            return new[] { $"{_template.GetTypeName(ServiceContractTemplate.IDENTIFIER, _template.Model)} appService" };
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
            result = await _appService.{operationModel.Name.ToPascalCase()}({_template.GetArguments(operationModel.Parameters)});";
            }
            return $@"
            await _appService.{operationModel.Name.ToPascalCase()}({_template.GetArguments(operationModel.Parameters)});";
        }

        public override string ExitOperationBody(OperationModel operationModel)
        {
            var httpResponse = "Ok";
            if (operationModel.Name.StartsWith("Create", StringComparison.InvariantCultureIgnoreCase))
            {
                httpResponse = "Created";
            }

            return $@"
            return {httpResponse}({(operationModel.ReturnType != null ? "result" : "")});";
        }
    }
}