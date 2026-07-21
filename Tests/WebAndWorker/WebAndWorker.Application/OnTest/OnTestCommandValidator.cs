using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace WebAndWorker.Application.OnTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OnTestCommandValidator : AbstractValidator<OnTestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public OnTestCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}