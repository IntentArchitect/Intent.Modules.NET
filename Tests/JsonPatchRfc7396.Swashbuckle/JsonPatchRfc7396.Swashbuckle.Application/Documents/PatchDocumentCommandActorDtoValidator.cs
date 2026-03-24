using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PatchDocumentCommandActorDtoValidator : AbstractValidator<PatchDocumentCommandActorDto>
    {
        [IntentManaged(Mode.Merge)]
        public PatchDocumentCommandActorDtoValidator()
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