using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Orders
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PatchOrderCommandDeliveryAddressDtoValidator : AbstractValidator<PatchOrderCommandDeliveryAddressDto>
    {
        [IntentManaged(Mode.Merge)]
        public PatchOrderCommandDeliveryAddressDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Line1)
                .NotNull();
        }
    }
}