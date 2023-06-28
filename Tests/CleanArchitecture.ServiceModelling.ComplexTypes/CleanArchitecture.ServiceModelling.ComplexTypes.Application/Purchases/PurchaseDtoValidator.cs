using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PurchaseDtoValidator : AbstractValidator<PurchaseDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public PurchaseDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Cost)
                .NotNull();
        }
    }
}