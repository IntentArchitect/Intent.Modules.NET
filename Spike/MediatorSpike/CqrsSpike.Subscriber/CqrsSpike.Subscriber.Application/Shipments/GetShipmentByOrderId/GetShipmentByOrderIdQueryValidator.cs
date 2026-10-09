using FluentValidation;

namespace CqrsSpike.Subscriber.Application.Shipments.GetShipmentByOrderId
{
    public class GetShipmentByOrderIdQueryValidator : AbstractValidator<GetShipmentByOrderIdQuery>
    {
        public GetShipmentByOrderIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}