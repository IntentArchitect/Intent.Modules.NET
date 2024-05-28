using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace TrainingModel.Tests.Application.Products.GetProductsSearch
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetProductsSearchQueryValidator : AbstractValidator<GetProductsSearchQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetProductsSearchQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.SearchTerm)
                .NotNull();
        }
    }
}