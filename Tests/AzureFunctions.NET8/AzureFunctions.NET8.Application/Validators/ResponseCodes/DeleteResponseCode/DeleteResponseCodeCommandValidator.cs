using AzureFunctions.NET8.Application.ResponseCodes.DeleteResponseCode;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Validators.ResponseCodes.DeleteResponseCode
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteResponseCodeCommandValidator : AbstractValidator<DeleteResponseCodeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteResponseCodeCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}