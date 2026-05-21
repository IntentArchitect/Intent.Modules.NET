using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.GetCustomerSegmentsById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomerSegmentsByIdQueryValidator : AbstractValidator<GetCustomerSegmentsByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomerSegmentsByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}