using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.ConfigurationStores
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PatchConfigurationStoreChangesDtoValidator : AbstractValidator<PatchConfigurationStoreChangesDto>
    {
        [IntentManaged(Mode.Merge)]
        public PatchConfigurationStoreChangesDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Key)
                .NotNull()
                .SetValidator(provider.GetValidator<PatchConfigurationStoreKeyDto>()!);

            RuleFor(v => v.ScopeKey)
                .NotNull()
                .SetValidator(provider.GetValidator<PatchConfigurationStoreScopeKeyDto>()!);

            RuleFor(v => v.ChangedBy)
                .NotNull();
        }
    }
}