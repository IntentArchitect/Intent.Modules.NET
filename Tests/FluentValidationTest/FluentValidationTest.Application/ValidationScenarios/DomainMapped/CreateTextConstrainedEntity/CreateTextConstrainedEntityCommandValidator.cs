using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateTextConstrainedEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateTextConstrainedEntityCommandValidator : AbstractValidator<CreateTextConstrainedEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateTextConstrainedEntityCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ShortCode)
                .NotNull()
                .MaximumLength(8)
                .MinimumLength(2);

            RuleFor(v => v.DisplayName)
                .NotNull()
                .MaximumLength(100)
                .MinimumLength(3);

            RuleFor(v => v.RequiredName)
                .NotNull()
                .NotEmpty();

            RuleFor(v => v.NullButRequired)
                .NotEmpty();

            RuleFor(v => v.DefaultIntButRequired)
                .NotEmpty();
        }
    }
}