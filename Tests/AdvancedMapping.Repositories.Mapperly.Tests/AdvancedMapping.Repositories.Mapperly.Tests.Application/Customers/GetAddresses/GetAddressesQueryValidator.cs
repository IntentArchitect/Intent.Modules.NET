using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.GetAddresses
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAddressesQueryValidator : AbstractValidator<GetAddressesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetAddressesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}