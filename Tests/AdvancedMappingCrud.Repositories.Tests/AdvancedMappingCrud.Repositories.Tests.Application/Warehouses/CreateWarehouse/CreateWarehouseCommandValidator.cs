using AdvancedMappingCrud.Repositories.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Warehouses.CreateWarehouse
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateWarehouseCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Address)
                .SetValidator(provider.GetValidator<CreateWarehouseCommandAddressDto>()!);
        }
    }
}