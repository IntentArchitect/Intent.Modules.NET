using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.ConfigurationStores
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateConfigurationStoreScopeKeyDtoValidator : AbstractValidator<UpdateConfigurationStoreScopeKeyDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateConfigurationStoreScopeKeyDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Scope)
                .NotNull()
                .IsInEnum();
        }
    }
}