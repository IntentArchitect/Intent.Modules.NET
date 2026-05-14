using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.GetCustomerSegments
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomerSegmentsQueryValidator : AbstractValidator<GetCustomerSegmentsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomerSegmentsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}