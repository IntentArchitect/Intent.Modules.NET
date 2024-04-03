using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Contracts.Services.Validation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ValidatedCommandValidator : AbstractValidator<ValidatedCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ValidatedCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Field)
                .NotNull();

            RuleFor(v => v.Email)
                .NotNull()
                .EmailAddress();

            RuleFor(v => v.CascaseTest)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .MinimumLength(1);
        }
    }
}