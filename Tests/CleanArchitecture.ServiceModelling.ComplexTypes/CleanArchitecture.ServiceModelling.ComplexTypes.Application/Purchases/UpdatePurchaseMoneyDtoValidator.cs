using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdatePurchaseMoneyDtoValidator : AbstractValidator<UpdatePurchaseMoneyDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdatePurchaseMoneyDtoValidator()
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