using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDocumentCommandChangesDtoValidator : AbstractValidator<UpdateDocumentCommandChangesDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDocumentCommandChangesDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.PatchJson)
                .NotNull();

            RuleFor(v => v.Actor)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateDocumentCommandActorDto>()!);

            RuleFor(v => v.ClientChangeId)
                .NotNull();
        }
    }
}