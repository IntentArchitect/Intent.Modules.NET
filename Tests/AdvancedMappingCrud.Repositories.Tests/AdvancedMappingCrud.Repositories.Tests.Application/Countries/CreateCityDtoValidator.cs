using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Countries
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCityDtoValidator : AbstractValidator<CreateCityDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCityDtoValidator()
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