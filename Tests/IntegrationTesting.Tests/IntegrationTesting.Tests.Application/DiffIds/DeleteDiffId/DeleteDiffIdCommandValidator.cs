using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.DiffIds.DeleteDiffId
{
    public class DeleteDiffIdCommandValidator : AbstractValidator<DeleteDiffIdCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteDiffIdCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}