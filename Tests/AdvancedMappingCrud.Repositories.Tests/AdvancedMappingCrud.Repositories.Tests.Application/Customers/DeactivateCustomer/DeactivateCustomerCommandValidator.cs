using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.DeactivateCustomer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeactivateCustomerCommandValidator : AbstractValidator<DeactivateCustomerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeactivateCustomerCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}