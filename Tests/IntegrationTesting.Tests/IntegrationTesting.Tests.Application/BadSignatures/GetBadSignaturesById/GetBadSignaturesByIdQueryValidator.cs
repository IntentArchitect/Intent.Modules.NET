using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.BadSignatures.GetBadSignaturesById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetBadSignaturesByIdQueryValidator : AbstractValidator<GetBadSignaturesByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetBadSignaturesByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}