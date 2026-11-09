using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.FluentValidation.CommandValidator", Version = "2.0")]

namespace TestMediator.Hangfire.Application.Maintenance.PurgeExpiredSessions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PurgeExpiredSessionsCommandValidator : AbstractValidator<PurgeExpiredSessionsCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PurgeExpiredSessionsCommandValidator()
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