using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDocumentTitleDtoValidator : AbstractValidator<UpdateDocumentTitleDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDocumentTitleDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Value)
                .NotNull();
        }
    }
}