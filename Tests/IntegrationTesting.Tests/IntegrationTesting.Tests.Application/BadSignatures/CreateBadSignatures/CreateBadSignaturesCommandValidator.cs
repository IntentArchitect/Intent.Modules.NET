using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.BadSignatures.CreateBadSignatures
{
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