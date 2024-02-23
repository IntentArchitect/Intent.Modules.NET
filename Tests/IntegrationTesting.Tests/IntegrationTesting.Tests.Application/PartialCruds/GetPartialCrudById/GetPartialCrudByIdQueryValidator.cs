using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.PartialCruds.GetPartialCrudById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPartialCrudByIdQueryValidator : AbstractValidator<GetPartialCrudByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPartialCrudByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}