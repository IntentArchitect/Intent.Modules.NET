using CleanArchitecture.Comprehensive.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Customers.UpdateCustomer
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
                .MaximumLength(100);

            RuleFor(v => v.Surname)
                .NotNull()
                .MaximumLength(100);

            RuleFor(v => v.Email)
                .NotNull()
                .MaximumLength(100);

            RuleFor(v => v.Address)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateCustomerAddressDto>()!);
        }
    }
}