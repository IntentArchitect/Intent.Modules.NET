using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.SimpleDownload
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SimpleDownloadQueryValidator : AbstractValidator<SimpleDownloadQuery>
    {
        [IntentManaged(Mode.Merge)]
        public SimpleDownloadQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}