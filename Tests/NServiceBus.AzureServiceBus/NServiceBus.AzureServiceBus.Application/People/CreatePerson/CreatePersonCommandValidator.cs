using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace NServiceBus.AzureServiceBus.Application.People.CreatePerson
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreatePersonCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}