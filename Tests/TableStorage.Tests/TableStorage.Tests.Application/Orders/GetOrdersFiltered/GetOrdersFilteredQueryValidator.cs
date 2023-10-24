using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace TableStorage.Tests.Application.Orders.GetOrdersFiltered
{
    public class GetOrdersFilteredQueryValidator : AbstractValidator<GetOrdersFilteredQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrdersFilteredQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.PartitionKey)
                .NotNull();
        }
    }
}