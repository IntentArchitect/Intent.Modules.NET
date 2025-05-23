using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Ardalis.Application.Clients.DeleteClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteClientCommandValidator : AbstractValidator<DeleteClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteClientCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}