using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.SimpleUpload
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SimpleUploadCommandValidator : AbstractValidator<SimpleUploadCommand>
    {
        [IntentManaged(Mode.Merge)]
        public SimpleUploadCommandValidator()
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