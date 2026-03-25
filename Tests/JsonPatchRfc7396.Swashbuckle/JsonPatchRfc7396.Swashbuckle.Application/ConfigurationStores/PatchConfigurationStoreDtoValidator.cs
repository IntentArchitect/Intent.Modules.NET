using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.ConfigurationStores
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PatchConfigurationStoreDtoValidator : AbstractValidator<PatchConfigurationStoreDto>
    {
        [IntentManaged(Mode.Merge)]
        public PatchConfigurationStoreDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Changes)
                .ForEach(x => x.SetValidator(provider.GetValidator<PatchConfigurationStoreChangesDto>()!));

            RuleFor(v => v.Items)
                .ForEach(x => x.SetValidator(provider.GetValidator<PatchConfigurationStoreItemsDto>()!));
        }
    }
}