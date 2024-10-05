using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Hangfire.Tests.Application.Job
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class JobCommandValidator : AbstractValidator<JobCommand>
    {
        [IntentManaged(Mode.Merge)]
        public JobCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}