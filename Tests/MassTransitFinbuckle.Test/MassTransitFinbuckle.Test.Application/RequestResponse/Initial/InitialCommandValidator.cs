using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MassTransitFinbuckle.Test.Application.RequestResponse.Initial
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InitialCommandValidator : AbstractValidator<InitialCommand>
    {
        [IntentManaged(Mode.Merge)]
        public InitialCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Value)
                .NotNull();
        }
    }
}