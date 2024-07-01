using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.RichProducts.GetRichProductById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetRichProductByIdQueryValidator : AbstractValidator<GetRichProductByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetRichProductByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}