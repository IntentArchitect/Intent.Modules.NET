using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CosmosDB.Application.Regions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
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