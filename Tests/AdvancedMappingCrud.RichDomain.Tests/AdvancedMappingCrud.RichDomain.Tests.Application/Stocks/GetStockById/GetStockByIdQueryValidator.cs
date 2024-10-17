using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Stocks.GetStockById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetStockByIdQueryValidator : AbstractValidator<GetStockByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetStockByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}