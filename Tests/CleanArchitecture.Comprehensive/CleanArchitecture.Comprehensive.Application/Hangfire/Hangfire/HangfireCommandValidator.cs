using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Hangfire.Hangfire
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class HangfireCommandValidator : AbstractValidator<HangfireCommand>
    {
        [IntentManaged(Mode.Merge)]
        public HangfireCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}