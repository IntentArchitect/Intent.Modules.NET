using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CqrsSpike.Subscriber.Application.Shipments.GetShipmentByOrderId
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetShipmentByOrderIdQueryValidator : AbstractValidator<GetShipmentByOrderIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetShipmentByOrderIdQueryValidator()
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