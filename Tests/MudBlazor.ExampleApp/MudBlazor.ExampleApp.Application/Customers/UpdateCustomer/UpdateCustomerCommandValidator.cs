using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Application.Customers.UpdateCustomer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCustomerCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull()
                .MaximumLength(40);

            RuleFor(v => v.AccountNo)
                .MaximumLength(20);

            RuleFor(v => v.Address)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateCustomerAddressDto>()!);
        }
    }
}