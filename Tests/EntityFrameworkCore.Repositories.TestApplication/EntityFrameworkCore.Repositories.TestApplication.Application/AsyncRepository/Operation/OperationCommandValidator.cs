using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.AsyncRepository.Operation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OperationCommandValidator : AbstractValidator<OperationCommand>
    {
        [IntentManaged(Mode.Merge)]
        public OperationCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}