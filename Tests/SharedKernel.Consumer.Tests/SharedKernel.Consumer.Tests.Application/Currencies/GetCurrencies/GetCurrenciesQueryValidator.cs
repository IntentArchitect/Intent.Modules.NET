using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.GetCurrencies
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCurrenciesQueryValidator : AbstractValidator<GetCurrenciesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCurrenciesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}