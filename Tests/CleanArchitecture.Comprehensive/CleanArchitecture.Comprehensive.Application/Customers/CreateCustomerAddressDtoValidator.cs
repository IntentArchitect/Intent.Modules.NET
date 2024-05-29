using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Customers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCustomerAddressDtoValidator : AbstractValidator<CreateCustomerAddressDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCustomerAddressDtoValidator()
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
        }
    }
}