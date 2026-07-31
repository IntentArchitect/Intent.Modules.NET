using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace WebAndWorker.Application.App.Orders.UploadOrderDocument
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UploadOrderDocumentCommandValidator : AbstractValidator<UploadOrderDocumentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UploadOrderDocumentCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.File)
                .NotNull();

            RuleFor(v => v.FileName)
                .NotNull();
        }
    }
}