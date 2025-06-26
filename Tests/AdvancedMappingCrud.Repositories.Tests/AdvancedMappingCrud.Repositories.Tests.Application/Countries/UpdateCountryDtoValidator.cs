using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Countries
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCountryDtoValidator : AbstractValidator<UpdateCountryDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCountryDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.MaE)
                .NotNull();
        }
    }
}