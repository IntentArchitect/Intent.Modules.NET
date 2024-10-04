using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Customers.ChangeName
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ChangeNameCommandValidator : AbstractValidator<ChangeNameCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ChangeNameCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull()
                .MaximumLength(100);

            RuleFor(v => v.Surname)
                .NotNull()
                .MaximumLength(100);
        }
    }
}