using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.DeleteAddress
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteAddressCommandValidator : AbstractValidator<DeleteAddressCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteAddressCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}