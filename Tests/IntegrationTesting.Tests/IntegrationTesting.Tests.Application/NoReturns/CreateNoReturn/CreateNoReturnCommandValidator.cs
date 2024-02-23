using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.NoReturns.CreateNoReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateNoReturnCommandValidator : AbstractValidator<CreateNoReturnCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateNoReturnCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}