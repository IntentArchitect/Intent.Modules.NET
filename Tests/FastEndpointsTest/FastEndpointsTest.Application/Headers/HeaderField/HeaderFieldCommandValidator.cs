using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.Headers.HeaderField
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class HeaderFieldCommandValidator : AbstractValidator<HeaderFieldCommand>
    {
        [IntentManaged(Mode.Merge)]
        public HeaderFieldCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Header)
                .NotNull();
        }
    }
}