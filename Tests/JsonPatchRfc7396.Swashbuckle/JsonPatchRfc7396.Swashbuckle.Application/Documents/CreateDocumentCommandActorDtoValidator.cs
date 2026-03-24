using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDocumentCommandActorDtoValidator : AbstractValidator<CreateDocumentCommandActorDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDocumentCommandActorDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.UserId)
                .NotNull();

            RuleFor(v => v.DisplayName)
                .NotNull();
        }
    }
}