using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.DeleteHasDateOnlyField
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteHasDateOnlyFieldCommandValidator : AbstractValidator<DeleteHasDateOnlyFieldCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteHasDateOnlyFieldCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}