using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.NoReturns.DeleteNoReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteNoReturnCommandValidator : AbstractValidator<DeleteNoReturnCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteNoReturnCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}