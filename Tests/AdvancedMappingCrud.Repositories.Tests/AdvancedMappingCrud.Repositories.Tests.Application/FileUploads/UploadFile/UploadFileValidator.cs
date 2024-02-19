using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.UploadFile
{
    public class UploadFileValidator : AbstractValidator<UploadFile>
    {
        [IntentManaged(Mode.Merge)]
        public UploadFileValidator()
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