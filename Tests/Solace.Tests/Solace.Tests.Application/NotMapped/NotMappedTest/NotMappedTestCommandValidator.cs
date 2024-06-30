using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Solace.Tests.Application.NotMapped.NotMappedTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class NotMappedTestCommandValidator : AbstractValidator<NotMappedTestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public NotMappedTestCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}