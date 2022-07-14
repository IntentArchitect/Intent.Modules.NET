using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.FluentValidation.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ValidatorHandlerDecorator : AzureFunctionClassDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AzureFunctions.FluentValidation.ValidatorHandlerDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly AzureFunctionClassTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        private readonly string _requestDtoTypeName;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValidatorHandlerDecorator(AzureFunctionClassTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _requestDtoTypeName = _template.GetRequestDtoType();
        }

        public override IEnumerable<string> GetClassEntryDefinitionList()
        {
            if (_requestDtoTypeName == null)
            {
                yield break;
            }

            yield return $"private readonly ValidationHandler<{_requestDtoTypeName}> _validation;";
        }

        public override IEnumerable<string> GetConstructorParameterDefinitionList()
        {
            if (_requestDtoTypeName == null)
            {
                yield break;
            }

            yield return $"ValidationHandler<{_requestDtoTypeName}> validation";
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

            yield return $"var failures = await _validation.Handle(dto);";
            yield return $"if (failures.Count > 0)";
            yield return $"{{";
            yield return $"    return new BadRequestObjectResult(failures);";
            yield return $"}}";
        }
    }
}