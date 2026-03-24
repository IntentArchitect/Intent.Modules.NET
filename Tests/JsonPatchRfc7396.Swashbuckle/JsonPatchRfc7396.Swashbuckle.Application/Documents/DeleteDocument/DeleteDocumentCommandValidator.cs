using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents.DeleteDocument
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteDocumentCommandValidator : AbstractValidator<DeleteDocumentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteDocumentCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}