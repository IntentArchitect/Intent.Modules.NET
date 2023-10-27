using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.Inheritance.ConcreteClasses.DeleteConcreteClass
{
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