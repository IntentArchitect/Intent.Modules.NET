using Intent.Engine;
using Intent.Modules.Application.FluentValidation.Templates.ValidationBehaviour;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.FluentValidation.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ValidationBehaviourDecorator : ValidationBehaviourContract
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AzureFunctions.FluentValidation.ValidationBehaviourDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly ValidationBehaviourTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public ValidationBehaviourDecorator(ValidationBehaviourTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }
    }
}