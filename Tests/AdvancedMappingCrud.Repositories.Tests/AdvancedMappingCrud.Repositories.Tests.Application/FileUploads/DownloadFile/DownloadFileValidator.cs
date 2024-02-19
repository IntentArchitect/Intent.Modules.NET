using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.DownloadFile
{
    public class DownloadFileValidator : AbstractValidator<DownloadFile>
    {
        [IntentManaged(Mode.Merge)]
        public DownloadFileValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}