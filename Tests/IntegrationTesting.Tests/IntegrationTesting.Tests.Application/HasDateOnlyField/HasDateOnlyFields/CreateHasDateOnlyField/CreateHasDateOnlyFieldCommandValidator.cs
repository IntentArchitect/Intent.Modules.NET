using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.CreateHasDateOnlyField
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateHasDateOnlyFieldCommandValidator : AbstractValidator<CreateHasDateOnlyFieldCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateHasDateOnlyFieldCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}