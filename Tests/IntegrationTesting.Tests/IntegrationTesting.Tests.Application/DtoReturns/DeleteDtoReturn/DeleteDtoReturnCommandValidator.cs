using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.DtoReturns.DeleteDtoReturn
{
    public class DeleteDtoReturnCommandValidator : AbstractValidator<DeleteDtoReturnCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteDtoReturnCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}