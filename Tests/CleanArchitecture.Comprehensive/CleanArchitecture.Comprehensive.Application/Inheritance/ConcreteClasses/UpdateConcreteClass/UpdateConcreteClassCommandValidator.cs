using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.UpdateConcreteClass
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateConcreteClassCommandValidator : AbstractValidator<UpdateConcreteClassCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateConcreteClassCommandValidator()
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