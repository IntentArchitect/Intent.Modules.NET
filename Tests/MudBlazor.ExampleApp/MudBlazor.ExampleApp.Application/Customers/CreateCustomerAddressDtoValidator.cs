using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Application.Customers
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
                .NotNull()
                .MaximumLength(200);

            RuleFor(v => v.Line2)
                .MaximumLength(200);

            RuleFor(v => v.City)
                .NotNull()
                .MaximumLength(100);

            RuleFor(v => v.Country)
                .NotNull()
                .MaximumLength(50);

            RuleFor(v => v.Postal)
                .NotNull()
                .MaximumLength(20);
        }
    }
}