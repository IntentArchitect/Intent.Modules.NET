using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modules.AzureFunctions.FluentValidation.Templates;
using Intent.Modules.AzureFunctions.FluentValidation.Templates.ValidationServiceInterface;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.FluentValidation.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class AzureFunctionFluentValidationDecorator : AzureFunctionClassDecorator, IDecoratorExecutionHooks
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AzureFunctions.FluentValidation.AzureFunctionFluentValidationDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly AzureFunctionClassTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        private readonly string _requestDtoTypeName;
        
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureFunctionFluentValidationDecorator(AzureFunctionClassTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _requestDtoTypeName = _template.Model.GetRequestDtoParameter() != null
                ? _template.GetTypeName(_template.Model.GetRequestDtoParameter().TypeReference)
                : null;
        }
        
        public void BeforeTemplateExecution()
        {
            if (_requestDtoTypeName == null)
            {
                return;
            }

            if (_template.Model.GetAzureFunction()?.Type().IsHttpTrigger() != true)
            {
                return;
            }

            var templateDependency = TemplateDependency.OnTemplate(ValidationServiceInterfaceTemplate.TemplateId);
            var template = _template.GetTemplate<IClassProvider>(templateDependency);
            if (template != null)
            {
                _template.AddUsing(template.Namespace);
            }

            _template.AddTemplateDependency(templateDependency);
        }

        public override IEnumerable<string> GetClassEntryDefinitionList()
        {
            if (_requestDtoTypeName == null)
            {
                yield break;
            }

            if (_template.Model.GetAzureFunction()?.Type().IsHttpTrigger() != true)
            {
                yield break;
            }

            yield return $"private readonly {_template.GetValidationServiceInterfaceName()} _validation;";
        }

        public override IEnumerable<string> GetConstructorParameterDefinitionList()
        {
            if (_requestDtoTypeName == null)
            {
                yield break;
            }

            if (_template.Model.GetAzureFunction()?.Type().IsHttpTrigger() != true)
            {
                yield break;
            }

            yield return $"{_template.GetValidationServiceInterfaceName()} validation";
        }

        public override IEnumerable<string> GetConstructorBodyStatementList()
        {
            if (_requestDtoTypeName == null)
            {
                yield break;
            }

            if (_template.Model.GetAzureFunction()?.Type().IsHttpTrigger() != true)
            {
                yield break;
            }

            yield return $"_validation = validation ?? throw new ArgumentNullException(nameof(validation));";
        }

        public override IEnumerable<string> GetRunMethodEntryStatementList()
        {
            if (_requestDtoTypeName == null)
            {
                yield break;
            }

            if (_template.Model.GetAzureFunction()?.Type().IsHttpTrigger() != true)
            {
                yield break;
            }

            yield return $"await _validation.Handle({_template.Model.GetRequestDtoParameter().Name.ToParameterName()}, default);";
        }

        public override IEnumerable<ExceptionCatchBlock> GetExceptionCatchBlocks()
        {
            if (_requestDtoTypeName == null)
            {
                yield break;
            }

            if (_template.Model.GetAzureFunction()?.Type().IsHttpTrigger() != true)
            {
                yield break;
            }

            yield return new ExceptionCatchBlock("ValidationException exception")
                .AddNamespaces("FluentValidation")
                .AddStatementLines("return new BadRequestObjectResult(exception.Errors);");
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            yield return NuGetPackages.FluentValidation;
        }
    }
}