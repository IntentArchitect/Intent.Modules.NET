using FluentValidation;

namespace CqrsSpike.Publisher.Application.AuditEntries.CreateAuditEntry
{
    public class CreateAuditEntryCommandValidator : AbstractValidator<CreateAuditEntryCommand>
    {
        public CreateAuditEntryCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}