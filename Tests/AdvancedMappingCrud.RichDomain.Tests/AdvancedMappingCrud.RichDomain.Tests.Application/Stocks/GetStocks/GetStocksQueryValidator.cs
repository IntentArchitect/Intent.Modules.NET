using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Stocks.GetStocks
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetStocksQueryValidator : AbstractValidator<GetStocksQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetStocksQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}