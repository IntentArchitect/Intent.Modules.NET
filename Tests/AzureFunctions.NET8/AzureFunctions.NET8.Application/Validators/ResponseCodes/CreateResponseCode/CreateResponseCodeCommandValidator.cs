using AzureFunctions.NET8.Application.ResponseCodes.CreateResponseCode;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Validators.ResponseCodes.CreateResponseCode
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateResponseCodeCommandValidator : AbstractValidator<CreateResponseCodeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateResponseCodeCommandValidator()
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