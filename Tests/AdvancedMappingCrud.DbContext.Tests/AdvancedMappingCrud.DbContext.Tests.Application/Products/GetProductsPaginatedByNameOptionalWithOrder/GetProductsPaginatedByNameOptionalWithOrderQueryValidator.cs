using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Products.GetProductsPaginatedByNameOptionalWithOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetProductsPaginatedByNameOptionalWithOrderQueryValidator : AbstractValidator<GetProductsPaginatedByNameOptionalWithOrderQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetProductsPaginatedByNameOptionalWithOrderQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.OrderBy)
                .NotNull();
        }
    }
}