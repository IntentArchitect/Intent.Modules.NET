using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.GetCustomersLean
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomersLeanQueryValidator : AbstractValidator<GetCustomersLeanQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomersLeanQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}