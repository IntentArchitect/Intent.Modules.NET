using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.BadSignatures.GetBadSignatures
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetBadSignaturesQueryValidator : AbstractValidator<GetBadSignaturesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetBadSignaturesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Filter)
                .NotNull();
        }
    }
}