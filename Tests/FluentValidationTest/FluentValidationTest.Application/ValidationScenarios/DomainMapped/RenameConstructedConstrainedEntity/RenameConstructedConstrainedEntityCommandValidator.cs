using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.RenameConstructedConstrainedEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RenameConstructedConstrainedEntityCommandValidator : AbstractValidator<RenameConstructedConstrainedEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public RenameConstructedConstrainedEntityCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.NewTitle)
                .NotNull();

            RuleFor(v => v.NewCode)
                .NotNull();
        }
    }
}