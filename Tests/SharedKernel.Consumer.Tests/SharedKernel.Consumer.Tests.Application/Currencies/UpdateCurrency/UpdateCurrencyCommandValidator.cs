using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.UpdateCurrency
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCurrencyCommandValidator : AbstractValidator<UpdateCurrencyCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCurrencyCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Symbol)
                .NotNull();

            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}