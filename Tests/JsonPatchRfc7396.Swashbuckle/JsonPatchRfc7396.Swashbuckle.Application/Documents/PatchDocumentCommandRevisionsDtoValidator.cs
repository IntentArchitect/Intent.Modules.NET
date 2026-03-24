using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PatchDocumentCommandRevisionsDtoValidator : AbstractValidator<PatchDocumentCommandRevisionsDto>
    {
        [IntentManaged(Mode.Merge)]
        public PatchDocumentCommandRevisionsDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Content)
                .NotNull()
                .SetValidator(provider.GetValidator<PatchDocumentCommandContentDto>()!);

            RuleFor(v => v.Author)
                .NotNull()
                .SetValidator(provider.GetValidator<PatchDocumentCommandActorDto>()!);
        }
    }
}