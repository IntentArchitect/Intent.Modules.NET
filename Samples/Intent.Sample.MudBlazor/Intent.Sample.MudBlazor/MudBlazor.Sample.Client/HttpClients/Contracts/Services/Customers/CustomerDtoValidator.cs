using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace MudBlazor.Sample.Client.HttpClients.Contracts.Services.Customers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomerDtoValidator : AbstractValidator<CustomerDto>
    {
        [IntentManaged(Mode.Merge)]
        public CustomerDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull()
                .MaximumLength(40);

            RuleFor(v => v.AccountNo)
                .MaximumLength(20);

            RuleFor(v => v.AddressLine1)
                .NotNull()
                .MaximumLength(200);

            RuleFor(v => v.AddressLine2)
                .MaximumLength(200);

            RuleFor(v => v.AddressCity)
                .NotNull()
                .MaximumLength(100);

            RuleFor(v => v.AddressCountry)
                .NotNull()
                .MaximumLength(50);

            RuleFor(v => v.AddressPostal)
                .NotNull()
                .MaximumLength(20);
        }
    }
}