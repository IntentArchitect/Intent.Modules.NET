using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Countries
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ChangeNameDtoValidator : AbstractValidator<ChangeNameDto>
    {
        [IntentManaged(Mode.Merge)]
        public ChangeNameDtoValidator()
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