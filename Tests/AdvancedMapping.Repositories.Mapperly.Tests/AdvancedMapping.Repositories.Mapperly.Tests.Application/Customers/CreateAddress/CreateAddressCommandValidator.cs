using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.CreateAddress
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAddressCommandValidator : AbstractValidator<CreateAddressCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAddressCommandValidator()
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

            RuleFor(v => v.Province)
                .NotNull();

            RuleFor(v => v.PostalCode)
                .NotNull();

            RuleFor(v => v.Country)
                .NotNull();
        }
    }
}