using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.CreateCurrency
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCurrencyCommandValidator : AbstractValidator<CreateCurrencyCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCurrencyCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Symbol)
                .NotNull();
        }
    }
}