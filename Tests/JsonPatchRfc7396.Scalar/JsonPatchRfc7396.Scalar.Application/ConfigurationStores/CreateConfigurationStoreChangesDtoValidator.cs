using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Scalar.Application.ConfigurationStores
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateConfigurationStoreChangesDtoValidator : AbstractValidator<CreateConfigurationStoreChangesDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateConfigurationStoreChangesDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Key)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateConfigurationStoreKeyDto>()!);

            RuleFor(v => v.ScopeKey)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateConfigurationStoreScopeKeyDto>()!);

            RuleFor(v => v.ChangedBy)
                .NotNull();
        }
    }
}