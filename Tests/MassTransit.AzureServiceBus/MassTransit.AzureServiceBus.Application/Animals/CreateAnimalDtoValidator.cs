using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.Animals
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAnimalDtoValidator : AbstractValidator<CreateAnimalDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAnimalDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Type)
                .NotNull();
        }
    }
}