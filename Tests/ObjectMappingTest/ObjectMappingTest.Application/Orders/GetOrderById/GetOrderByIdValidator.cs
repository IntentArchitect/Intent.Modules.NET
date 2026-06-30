using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace ObjectMappingTest.Application.Orders.GetOrderById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOrderByIdValidator : AbstractValidator<GetOrderById>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderByIdValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}