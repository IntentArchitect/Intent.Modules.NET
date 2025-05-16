using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace ProxyServiceTests.OriginalServices.Application.File.FileUpload
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class FileUploadCommandValidator : AbstractValidator<FileUploadCommand>
    {
        [IntentManaged(Mode.Merge)]
        public FileUploadCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Content)
                .NotNull();
        }
    }
}