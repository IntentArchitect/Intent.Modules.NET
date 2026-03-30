using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdateTextConstrainedEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateTextConstrainedEntityCommandValidator : AbstractValidator<UpdateTextConstrainedEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateTextConstrainedEntityCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.DisplayName)
                .NotNull()
                .MaximumLength(100)
                .MinimumLength(3);

            RuleFor(v => v.ShortCode)
                .NotNull()
                .MaximumLength(8)
                .MinimumLength(2);

            RuleFor(v => v.RequiredName)
                .NotNull()
                .NotEmpty();
        }
    }
}