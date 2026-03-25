using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDocumentContentDtoValidator : AbstractValidator<UpdateDocumentContentDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDocumentContentDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Format)
                .NotNull();

            RuleFor(v => v.Text)
                .NotNull();

            RuleFor(v => v.Json)
                .NotNull();
        }
    }
}