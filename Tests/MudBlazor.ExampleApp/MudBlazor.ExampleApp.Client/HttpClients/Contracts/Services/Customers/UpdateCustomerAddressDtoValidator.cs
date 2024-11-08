using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCustomerAddressDtoValidator : AbstractValidator<UpdateCustomerAddressDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCustomerAddressDtoValidator()
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