using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace TrainingModel.Tests.Application.Customers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCustomerCommandAddressDtoValidator : AbstractValidator<UpdateCustomerCommandAddressDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCustomerCommandAddressDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Line1)
                .NotNull();

            RuleFor(v => v.Line2)
                .NotNull();

            RuleFor(v => v.City)
                .NotNull();

            RuleFor(v => v.Postal)
                .NotNull();

            RuleFor(v => v.AddressType)
                .NotNull()
                .IsInEnum();
        }
    }
}