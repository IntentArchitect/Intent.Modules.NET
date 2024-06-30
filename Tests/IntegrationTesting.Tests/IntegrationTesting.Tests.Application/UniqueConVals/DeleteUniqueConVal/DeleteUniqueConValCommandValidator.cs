using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.UniqueConVals.DeleteUniqueConVal
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteUniqueConValCommandValidator : AbstractValidator<DeleteUniqueConValCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteUniqueConValCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}