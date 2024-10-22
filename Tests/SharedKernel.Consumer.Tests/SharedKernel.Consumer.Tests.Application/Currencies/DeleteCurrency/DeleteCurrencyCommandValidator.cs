using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.DeleteCurrency
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteCurrencyCommandValidator : AbstractValidator<DeleteCurrencyCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteCurrencyCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}