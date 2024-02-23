using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.BadSignatures.DeleteBadSignatures
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteBadSignaturesCommandValidator : AbstractValidator<DeleteBadSignaturesCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteBadSignaturesCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.More)
                .NotNull();
        }
    }
}