using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrderCommandShipmentsDtoValidator : AbstractValidator<CreateOrderCommandShipmentsDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandShipmentsDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Dispatch)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateOrderCommandDispatchDto>()!);

            RuleFor(v => v.Manifest)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateOrderCommandManifestDto>()!);

            RuleFor(v => v.Provider)
                .NotNull();
        }
    }
}