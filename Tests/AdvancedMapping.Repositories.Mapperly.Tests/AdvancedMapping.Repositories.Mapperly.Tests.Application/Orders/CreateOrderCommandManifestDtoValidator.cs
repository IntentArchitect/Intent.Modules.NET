using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrderCommandManifestDtoValidator : AbstractValidator<CreateOrderCommandManifestDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandManifestDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.CarrierCode)
                .NotNull();

            RuleFor(v => v.Document)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateOrderCommandManifestDocumentDto>()!);
        }
    }
}