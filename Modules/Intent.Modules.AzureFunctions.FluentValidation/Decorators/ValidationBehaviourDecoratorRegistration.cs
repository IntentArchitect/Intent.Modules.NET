using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Application.FluentValidation.Templates.ValidationBehaviour;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.FluentValidation.Decorators
{
    [Description(ValidationBehaviourDecorator.DecoratorId)]
    public class ValidationBehaviourDecoratorRegistration : DecoratorRegistration<ValidationBehaviourTemplate, ValidationBehaviourContract>
    {
        public override ValidationBehaviourContract CreateDecoratorInstance(ValidationBehaviourTemplate template, IApplication application)
        {
            return new ValidationBehaviourDecorator(template, application);
        }

        public override string DecoratorId => ValidationBehaviourDecorator.DecoratorId;
    }
}