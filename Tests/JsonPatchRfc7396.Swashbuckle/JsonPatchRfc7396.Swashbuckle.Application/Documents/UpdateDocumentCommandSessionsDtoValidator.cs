using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDocumentCommandSessionsDtoValidator : AbstractValidator<UpdateDocumentCommandSessionsDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDocumentCommandSessionsDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.ConnectionId)
                .NotNull();

            RuleFor(v => v.CursorJson)
                .NotNull();

            RuleFor(v => v.SelectionJson)
                .NotNull();
        }
    }
}