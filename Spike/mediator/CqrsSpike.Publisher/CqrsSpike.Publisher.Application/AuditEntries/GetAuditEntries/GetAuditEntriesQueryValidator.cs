using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.FluentValidation.QueryValidator", Version = "2.0")]

namespace CqrsSpike.Publisher.Application.AuditEntries.GetAuditEntries
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAuditEntriesQueryValidator : AbstractValidator<GetAuditEntriesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetAuditEntriesQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}