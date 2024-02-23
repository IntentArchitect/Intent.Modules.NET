using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.UploadFile
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UploadFileCommandValidator()
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