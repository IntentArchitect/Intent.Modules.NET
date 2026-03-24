using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDocumentCommandChangesDtoValidator : AbstractValidator<CreateDocumentCommandChangesDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDocumentCommandChangesDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.PatchJson)
                .NotNull();

            RuleFor(v => v.Actor)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateDocumentCommandActorDto>()!);

            RuleFor(v => v.ClientChangeId)
                .NotNull();
        }
    }
}