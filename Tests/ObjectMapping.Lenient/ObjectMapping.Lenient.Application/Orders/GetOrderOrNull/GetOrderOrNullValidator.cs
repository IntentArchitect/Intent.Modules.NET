using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace ObjectMapping.Lenient.Application.Orders.GetOrderOrNull
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOrderOrNullValidator : AbstractValidator<GetOrderOrNull>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderOrNullValidator()
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