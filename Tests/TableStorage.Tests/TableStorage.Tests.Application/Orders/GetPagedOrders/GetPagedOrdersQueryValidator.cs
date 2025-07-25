using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace TableStorage.Tests.Application.Orders.GetPagedOrders
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPagedOrdersQueryValidator : AbstractValidator<GetPagedOrdersQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPagedOrdersQueryValidator()
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