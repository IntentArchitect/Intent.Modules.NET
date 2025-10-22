using AzureFunctions.NET8.Application.ResponseCodes.UpdateResponseCode;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Validators.ResponseCodes.UpdateResponseCode
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateResponseCodeCommandValidator : AbstractValidator<UpdateResponseCodeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateResponseCodeCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}