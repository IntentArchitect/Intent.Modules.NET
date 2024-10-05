using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.GetHasDateOnlyFieldById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetHasDateOnlyFieldByIdQueryValidator : AbstractValidator<GetHasDateOnlyFieldByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetHasDateOnlyFieldByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}