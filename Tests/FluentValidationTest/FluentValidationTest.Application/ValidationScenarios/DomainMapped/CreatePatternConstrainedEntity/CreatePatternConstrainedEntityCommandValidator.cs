using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreatePatternConstrainedEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreatePatternConstrainedEntityCommandValidator : AbstractValidator<CreatePatternConstrainedEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreatePatternConstrainedEntityCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.EmailAddress)
                .NotNull()
                .EmailAddress();

            RuleFor(v => v.Slug)
                .NotNull()
                .Matches(@"^[a-z0-9-]+$")
                .WithMessage(@"Slug must contain lowercase letters, digits or hyphens only.");

            RuleFor(v => v.ReferenceNumber)
                .NotNull()
                .Matches(@"^[A-Z]{3}-[0-9]{4}$")
                .WithMessage(@"ReferenceNumber must match AAA-1234 format.");
        }
    }
}