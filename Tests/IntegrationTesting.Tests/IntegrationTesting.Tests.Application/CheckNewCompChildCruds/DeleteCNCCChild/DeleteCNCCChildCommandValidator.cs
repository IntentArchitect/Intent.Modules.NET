using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.DeleteCNCCChild
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteCNCCChildCommandValidator : AbstractValidator<DeleteCNCCChildCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteCNCCChildCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}