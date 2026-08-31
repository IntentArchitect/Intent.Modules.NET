using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.FluentValidation.QueryValidator", Version = "2.0")]

namespace WolverineEventing.Coexist.Cqrs.Application.Orders.GetExistingOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetExistingOrderQueryValidator : AbstractValidator<GetExistingOrderQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetExistingOrderQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}