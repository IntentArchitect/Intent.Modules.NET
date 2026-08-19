using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Countries
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCityDtoValidator : AbstractValidator<CreateCityDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCityDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}