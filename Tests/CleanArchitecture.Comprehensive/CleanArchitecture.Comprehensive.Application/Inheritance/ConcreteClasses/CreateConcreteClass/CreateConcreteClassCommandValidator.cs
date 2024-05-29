using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.CreateConcreteClass
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateConcreteClassCommandValidator : AbstractValidator<CreateConcreteClassCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateConcreteClassCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ConcreteAttr)
                .NotNull();

            RuleFor(v => v.BaseAttr)
                .NotNull();
        }
    }
}