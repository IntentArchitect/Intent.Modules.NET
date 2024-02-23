using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.DownloadFile
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DownloadFileQueryValidator : AbstractValidator<DownloadFileQuery>
    {
        [IntentManaged(Mode.Merge)]
        public DownloadFileQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}