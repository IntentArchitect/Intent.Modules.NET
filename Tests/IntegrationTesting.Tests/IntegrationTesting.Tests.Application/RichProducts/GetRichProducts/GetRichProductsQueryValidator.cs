using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.RichProducts.GetRichProducts
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetRichProductsQueryValidator : AbstractValidator<GetRichProductsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetRichProductsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}