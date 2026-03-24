using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.ConfigurationStores
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateConfigurationStoreItemsDtoValidator : AbstractValidator<UpdateConfigurationStoreItemsDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateConfigurationStoreItemsDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Key)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateConfigurationStoreKeyDto>()!);

            RuleFor(v => v.ScopeKey)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateConfigurationStoreScopeKeyDto>()!);

            RuleFor(v => v.ValueType)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.Value)
                .NotNull();
        }
    }
}