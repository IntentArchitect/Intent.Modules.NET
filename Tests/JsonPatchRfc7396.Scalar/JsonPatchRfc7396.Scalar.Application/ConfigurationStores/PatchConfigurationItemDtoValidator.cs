using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Scalar.Application.ConfigurationStores
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PatchConfigurationItemDtoValidator : AbstractValidator<PatchConfigurationItemDto>
    {
        [IntentManaged(Mode.Merge)]
        public PatchConfigurationItemDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Key)
                .NotNull()
                .SetValidator(provider.GetValidator<PatchConfigurationItemKeyDto1>()!);

            RuleFor(v => v.ScopeKey)
                .NotNull()
                .SetValidator(provider.GetValidator<PatchConfigurationItemScopeKeyDto1>()!);

            RuleFor(v => v.ValueType)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.Value)
                .NotNull();
        }
    }
}