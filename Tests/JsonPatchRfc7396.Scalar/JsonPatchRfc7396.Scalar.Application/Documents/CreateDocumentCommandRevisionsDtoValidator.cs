using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDocumentCommandRevisionsDtoValidator : AbstractValidator<CreateDocumentCommandRevisionsDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDocumentCommandRevisionsDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Content)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateDocumentCommandContentDto>()!);

            RuleFor(v => v.Author)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateDocumentCommandActorDto>()!);
        }
    }
}