using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Children.DeleteChild
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteChildCommandValidator : AbstractValidator<DeleteChildCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteChildCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}