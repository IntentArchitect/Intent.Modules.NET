using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.GetHasDateOnlyFields
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetHasDateOnlyFieldsQueryValidator : AbstractValidator<GetHasDateOnlyFieldsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetHasDateOnlyFieldsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}