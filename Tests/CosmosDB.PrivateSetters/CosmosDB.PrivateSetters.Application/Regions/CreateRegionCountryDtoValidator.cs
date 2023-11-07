using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Regions
{
    public class CreateRegionCountryDtoValidator : AbstractValidator<CreateRegionCountryDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateRegionCountryDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}