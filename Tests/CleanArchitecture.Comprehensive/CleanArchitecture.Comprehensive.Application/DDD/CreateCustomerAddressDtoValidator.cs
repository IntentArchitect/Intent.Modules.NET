using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCustomerAddressDtoValidator : AbstractValidator<CreateCustomerAddressDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CreateCustomerAddressDtoValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
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