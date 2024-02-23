using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.BadSignatures.CreateBadSignatures
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateBadSignaturesCommandValidator : AbstractValidator<CreateBadSignaturesCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateBadSignaturesCommandValidator()
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