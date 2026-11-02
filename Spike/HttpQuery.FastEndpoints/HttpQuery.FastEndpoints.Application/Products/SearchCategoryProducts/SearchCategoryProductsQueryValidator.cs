using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace HttpQuery.FastEndpoints.Application.Products.SearchCategoryProducts
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SearchCategoryProductsQueryValidator : AbstractValidator<SearchCategoryProductsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public SearchCategoryProductsQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Category)
                .NotNull();
        }
    }
}