using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Pets.DeletePet
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeletePetCommandValidator : AbstractValidator<DeletePetCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeletePetCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Api_key)
                .NotNull();
        }
    }
}