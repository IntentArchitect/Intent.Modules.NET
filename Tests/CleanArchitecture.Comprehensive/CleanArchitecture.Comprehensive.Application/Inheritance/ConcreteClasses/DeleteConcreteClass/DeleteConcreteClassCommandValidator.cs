using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.DeleteConcreteClass
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteConcreteClassCommandValidator : AbstractValidator<DeleteConcreteClassCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteConcreteClassCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}