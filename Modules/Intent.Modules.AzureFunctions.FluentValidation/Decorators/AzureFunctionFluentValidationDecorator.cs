using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Application.FluentValidation.Templates.ValidationBehaviour;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common;
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
        [IntentManaged(Mode.Fully)] public const string DecoratorId =
            "Intent.AzureFunctions.FluentValidation.AzureFunctionFluentValidationDecorator";

        [IntentManaged(Mode.Fully)] private readonly AzureFunctionClassTemplate _template;
        [IntentManaged(Mode.Fully)] private readonly IApplication _application;

        private readonly string _requestDtoTypeName;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public AzureFunctionFluentValidationDecorator(AzureFunctionClassTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _requestDtoTypeName = _template.GetRequestDtoType();
        }

        public void BeforeTemplateExecution()
        {
            if (_requestDtoTypeName == null)
            {
                return;
            }
            
            var templateDependency = TemplateDependency.OnTemplate(ValidationBehaviourTemplate.TemplateId);
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

            yield return $"private readonly ValidationBehaviour<{_requestDtoTypeName}> _validation;";
        }

        public override IEnumerable<string> GetConstructorParameterDefinitionList()
        {
            if (_requestDtoTypeName == null)
            {
                yield break;
            }

            yield return $"ValidationBehaviour<{_requestDtoTypeName}> validation";
        }

        public override IEnumerable<string> GetConstructorBodyStatementList()
        {
            if (_requestDtoTypeName == null)
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

            yield return $"await _validation.Handle(dto, default);";
        }

        public override IEnumerable<ExceptionCatchBlock> GetExceptionCatchBlocks()
        {
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