using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.AzureFunctions.Templates;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Interop.Contracts.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ContractDispatchDecorator : AzureFunctionClassDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AzureFunctions.Interop.Contracts.ContractDispatchDecorator";

        [IntentManaged(Mode.Fully)] private readonly AzureFunctionClassTemplate _template;
        [IntentManaged(Mode.Fully)] private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public ContractDispatchDecorator(AzureFunctionClassTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override IEnumerable<string> GetClassEntryDefinitionList()
        {
            yield return
                $"private readonly {_template.GetTypeName(ServiceContractTemplate.TemplateId, _template.Model.ParentService)} _appService;";
        }

        public override IEnumerable<string> GetConstructorParameterDefinitionList()
        {
            yield return $"{_template.GetTypeName(ServiceContractTemplate.TemplateId, _template.Model.ParentService)} appService";
        }

        public override IEnumerable<string> GetConstructorBodyStatementList()
        {
            yield return "_appService = appService ?? throw new ArgumentNullException(nameof(appService));";
        }

        public override IEnumerable<string> GetRunMethodEntryStatementList()
        {
            if (_template.Model.ReturnType != null)
            {
                yield return $"var result = default({_template.GetTypeName(_template.Model.ReturnType)});";
            }
        }

        public override IEnumerable<string> GetRunMethodBodyStatementList()
        {
            if (_template.Model.ReturnType != null)
            {
                yield return
                    $"result = await _appService.{_template.Model.Name.ToPascalCase()}({_template.GetArguments(_template.Model.Parameters)});";
                yield break;
            }

            yield return
                $"await _appService.{_template.Model.Name.ToPascalCase()}({_template.GetArguments(_template.Model.Parameters)});";
        }

        public override IEnumerable<string> GetRunMethodExitStatementList()
        {
            if (_template.Model.GetAzureFunction()?.Type().IsHttpTrigger() != true)
            {
                return Enumerable.Empty<string>();
            }

            var httpTriggersView = _template.Model.GetAzureFunction().GetHttpTriggerView();
            string result = httpTriggersView.Method().AsEnum() switch
            {
                OperationModelStereotypeExtensions.AzureFunction.MethodOptionsEnum.GET => _template.Model.ReturnType ==
                    null
                        ? $"return new NoContentResult();"
                        : $"return new OkObjectResult({GetResultExpression()});",
                OperationModelStereotypeExtensions.AzureFunction.MethodOptionsEnum.POST =>
                    _template.Model.ReturnType == null
                        ? $"return new CreatedResult(string.Empty, null);"
                        : $"return new CreatedResult(string.Empty, {GetResultExpression()});",
                OperationModelStereotypeExtensions.AzureFunction.MethodOptionsEnum.PUT or
                    OperationModelStereotypeExtensions.AzureFunction.MethodOptionsEnum.PATCH => _template.Model.ReturnType ==
                    null
                        ? $"return new NoContentResult();"
                        : $"return new OkObjectResult({GetResultExpression()});",
                OperationModelStereotypeExtensions.AzureFunction.MethodOptionsEnum.DELETE => _template.Model.ReturnType ==
                    null
                        ? $"return new OkResult();"
                        : $"return new OkObjectResult({GetResultExpression()});",
                _ => throw new ArgumentOutOfRangeException()
            };

            return new[] { result };
        }
        
        private string GetResultExpression()
        {
            if (_template.Model.ReturnType == null)
            {
                throw new ArgumentException($@"{nameof(_template.Model.ReturnType)} is expected to be specified with a Type");
            }

            if (_template.Model.GetAzureFunction().ReturnTypeMediatype().IsApplicationJson()
                && _template.GetTypeInfo(_template.Model.ReturnType).IsPrimitive)
            {
                return $@"new {_template.GetJsonResponseName()}<{_template.GetTypeName(_template.Model.ReturnType)}>(result)";
            }

            return "result";
        }
    }
}