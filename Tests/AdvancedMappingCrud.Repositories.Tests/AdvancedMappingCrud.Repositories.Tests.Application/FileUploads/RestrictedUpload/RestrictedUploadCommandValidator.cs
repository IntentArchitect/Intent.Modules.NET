using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.RestrictedUpload
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RestrictedUploadCommandValidator : AbstractValidator<RestrictedUploadCommand>
    {
        [IntentManaged(Mode.Merge)]
        public RestrictedUploadCommandValidator()
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