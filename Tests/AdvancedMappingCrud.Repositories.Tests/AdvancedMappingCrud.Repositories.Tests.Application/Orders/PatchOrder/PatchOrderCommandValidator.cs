using AdvancedMappingCrud.Repositories.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Orders.PatchOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PatchOrderCommandValidator : AbstractValidator<PatchOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PatchOrderCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.OrderStatus)
                .IsInEnum();

            RuleFor(v => v.BillingAddress)
                .SetValidator(provider.GetValidator<PatchOrderCommandBillingAddressDto>()!);

            RuleFor(v => v.DeliveryAddress)
                .SetValidator(provider.GetValidator<PatchOrderCommandDeliveryAddressDto>()!);
        }
    }
}