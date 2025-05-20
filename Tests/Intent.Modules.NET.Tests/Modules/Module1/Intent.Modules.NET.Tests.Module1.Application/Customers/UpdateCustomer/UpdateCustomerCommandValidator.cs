using FluentValidation;
using Intent.Modules.NET.Tests.Module1.Application.Contracts.Customers.UpdateCustomer;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Customers.UpdateCustomer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCustomerCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}