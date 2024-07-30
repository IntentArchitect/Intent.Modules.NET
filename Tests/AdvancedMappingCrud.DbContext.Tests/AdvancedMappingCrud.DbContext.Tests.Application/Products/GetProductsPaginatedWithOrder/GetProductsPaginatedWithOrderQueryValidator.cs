using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Products.GetProductsPaginatedWithOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetProductsPaginatedWithOrderQueryValidator : AbstractValidator<GetProductsPaginatedWithOrderQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetProductsPaginatedWithOrderQueryValidator()
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