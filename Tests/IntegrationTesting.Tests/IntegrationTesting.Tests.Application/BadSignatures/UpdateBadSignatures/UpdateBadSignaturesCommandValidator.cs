using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.BadSignatures.UpdateBadSignatures
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateBadSignaturesCommandValidator : AbstractValidator<UpdateBadSignaturesCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateBadSignaturesCommandValidator()
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