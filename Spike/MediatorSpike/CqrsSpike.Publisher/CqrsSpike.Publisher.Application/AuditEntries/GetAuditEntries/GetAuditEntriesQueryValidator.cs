using FluentValidation;

namespace CqrsSpike.Publisher.Application.AuditEntries.GetAuditEntries
{
    public class GetAuditEntriesQueryValidator : AbstractValidator<GetAuditEntriesQuery>
    {
        public GetAuditEntriesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}