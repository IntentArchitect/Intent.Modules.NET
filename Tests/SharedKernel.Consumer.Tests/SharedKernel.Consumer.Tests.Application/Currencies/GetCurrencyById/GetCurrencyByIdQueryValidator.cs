using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies.GetCurrencyById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCurrencyByIdQueryValidator : AbstractValidator<GetCurrencyByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCurrencyByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}