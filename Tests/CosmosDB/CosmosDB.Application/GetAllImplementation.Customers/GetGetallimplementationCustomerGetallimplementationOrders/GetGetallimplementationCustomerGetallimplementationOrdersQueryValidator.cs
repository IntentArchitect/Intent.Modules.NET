using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.Application.GetAllImplementation.Customers.GetGetallimplementationCustomerGetallimplementationOrders
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetGetallimplementationCustomerGetallimplementationOrdersQueryValidator : AbstractValidator<GetGetallimplementationCustomerGetallimplementationOrdersQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetGetallimplementationCustomerGetallimplementationOrdersQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.GetallimplementationCustomerid)
                .NotNull();
        }
    }
}