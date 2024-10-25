using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Customers.ChangeEmailCustomer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ChangeEmailCustomerCommandValidator : AbstractValidator<ChangeEmailCustomerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ChangeEmailCustomerCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Email)
                .NotNull();
        }
    }
}