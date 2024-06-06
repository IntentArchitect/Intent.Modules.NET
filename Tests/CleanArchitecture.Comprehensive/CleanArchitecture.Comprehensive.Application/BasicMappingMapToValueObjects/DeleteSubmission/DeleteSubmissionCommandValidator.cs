using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects.DeleteSubmission
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteSubmissionCommandValidator : AbstractValidator<DeleteSubmissionCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteSubmissionCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}