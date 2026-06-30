using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace ObjectMappingTest.Application.Orders.GetOrderDetail
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOrderDetailValidator : AbstractValidator<GetOrderDetail>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderDetailValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}