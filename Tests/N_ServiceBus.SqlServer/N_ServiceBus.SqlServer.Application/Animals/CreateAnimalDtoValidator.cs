using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace N_ServiceBus.SqlServer.Application.Animals
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAnimalDtoValidator : AbstractValidator<CreateAnimalDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAnimalDtoValidator()
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