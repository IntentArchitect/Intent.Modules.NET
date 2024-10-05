using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.UpdateHasDateOnlyField
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateHasDateOnlyFieldCommandValidator : AbstractValidator<UpdateHasDateOnlyFieldCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateHasDateOnlyFieldCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}