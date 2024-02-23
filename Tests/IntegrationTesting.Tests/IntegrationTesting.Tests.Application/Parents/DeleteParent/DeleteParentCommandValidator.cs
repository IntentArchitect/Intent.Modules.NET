using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Parents.DeleteParent
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteParentCommandValidator : AbstractValidator<DeleteParentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteParentCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}