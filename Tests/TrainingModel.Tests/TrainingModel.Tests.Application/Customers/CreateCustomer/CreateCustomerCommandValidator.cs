using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace TrainingModel.Tests.Application.Customers.CreateCustomer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCustomerCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull()
                .MaximumLength(100);

            RuleFor(v => v.Surname)
                .NotNull();

            RuleFor(v => v.Email)
                .NotNull()
                .EmailAddress();

            RuleFor(v => v.Address)
                .NotNull()
                .CustomAsync(ValidateAddressAsync)
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateCustomerCommandAddressDto>()!));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        private async Task ValidateAddressAsync(
            IEnumerable<CreateCustomerCommandAddressDto> value,
            ValidationContext<CreateCustomerCommand> validationContext,
            CancellationToken cancellationToken)
        {
            if (!value.Any(a => a.AddressType == Domain.AddressType.Delivery))
            {
                validationContext.AddFailure("Require a delivery address");
            }
        }
    }
}