using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreatePurchaseMoneyDtoValidator : AbstractValidator<CreatePurchaseMoneyDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreatePurchaseMoneyDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Currency)
                .NotNull();
        }
    }
}