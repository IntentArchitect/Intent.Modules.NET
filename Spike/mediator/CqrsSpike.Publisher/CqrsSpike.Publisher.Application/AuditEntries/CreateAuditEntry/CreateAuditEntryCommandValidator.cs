using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.FluentValidation.CommandValidator", Version = "2.0")]

namespace CqrsSpike.Publisher.Application.AuditEntries.CreateAuditEntry
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAuditEntryCommandValidator : AbstractValidator<CreateAuditEntryCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAuditEntryCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}