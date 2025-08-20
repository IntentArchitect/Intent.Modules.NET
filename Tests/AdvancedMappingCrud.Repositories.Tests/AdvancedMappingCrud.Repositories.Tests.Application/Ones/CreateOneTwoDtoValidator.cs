using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Ones
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOneTwoDtoValidator : AbstractValidator<CreateOneTwoDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOneTwoDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.TwoName)
                .NotNull();
        }
    }
}