using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Templates;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.AzureFunctions.Templates;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common.CSharp.Builder;
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
            var mappedOperation = _template.Model.IsMapped ? _template.Model.Mapping.Element.AsOperationModel() : null;
            if (mappedOperation == null)
            {
                return;
            }

            _template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("FluentValidation");
                var @class = file.Classes.Single();
                @class.Constructors.First().AddParameter(_template.GetServiceContractName(mappedOperation.ParentService), "appService", param =>
                {
                    param.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                });

                var runMethod = @class.FindMethod("Run");
                runMethod.FindStatement<CSharpTryBlock>(x => true)?
                    .AddStatement($"{(_template.Model.ReturnType != null ? "var result = " : "")}await _appService.{_template.Model.Name.ToPascalCase()}({_template.GetArguments(_template.Model.Parameters)});",
                        statement => statement.AddMetadata("service-dispatch-statement", true))
                    .AddStatement(GetReturnStatement());
                //runMethod.FindStatement(x => x.GetText(string.Empty).Contains("await _appService"))
                //    .InsertAbove($"await _validation.Handle({_template.Model.GetRequestDtoParameter().Name.ToParameterName()}, default);");
            });
        }

        public CSharpStatement GetReturnStatement()
        {
            if (_template.Model.GetAzureFunction()?.Type().IsHttpTrigger() != true)
            {
                return null;
            }

            var httpTriggersView = _template.Model.GetAzureFunction().GetHttpTriggerView();
            string result = httpTriggersView.Method().AsEnum() switch
            {
                AzureFunctionModelStereotypeExtensions.AzureFunction.MethodOptionsEnum.GET => _template.Model.ReturnType ==
                    null
                        ? $"return new NoContentResult();"
                        : $"return new OkObjectResult({GetResultExpression()});",
                AzureFunctionModelStereotypeExtensions.AzureFunction.MethodOptionsEnum.POST =>
                    _template.Model.ReturnType == null
                        ? $"return new CreatedResult(string.Empty, null);"
                        : $"return new CreatedResult(string.Empty, {GetResultExpression()});",
                AzureFunctionModelStereotypeExtensions.AzureFunction.MethodOptionsEnum.PUT or
                    AzureFunctionModelStereotypeExtensions.AzureFunction.MethodOptionsEnum.PATCH => _template.Model.ReturnType ==
                    null
                        ? $"return new NoContentResult();"
                        : $"return new OkObjectResult({GetResultExpression()});",
                AzureFunctionModelStereotypeExtensions.AzureFunction.MethodOptionsEnum.DELETE => _template.Model.ReturnType ==
                    null
                        ? $"return new OkResult();"
                        : $"return new OkObjectResult({GetResultExpression()});",
                _ => throw new ArgumentOutOfRangeException()
            };

            return result;
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