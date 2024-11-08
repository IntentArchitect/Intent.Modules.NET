using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Application.Products.GetProductsLookup
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetProductsLookupQueryValidator : AbstractValidator<GetProductsLookupQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetProductsLookupQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}