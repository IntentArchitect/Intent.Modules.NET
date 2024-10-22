using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.GetDefaultCurrency
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDefaultCurrencyQueryValidator : AbstractValidator<GetDefaultCurrencyQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDefaultCurrencyQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}