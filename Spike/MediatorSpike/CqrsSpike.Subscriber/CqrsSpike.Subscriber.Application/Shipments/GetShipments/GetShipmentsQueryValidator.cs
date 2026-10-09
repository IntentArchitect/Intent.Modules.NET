using FluentValidation;

namespace CqrsSpike.Subscriber.Application.Shipments.GetShipments
{
    public class GetShipmentsQueryValidator : AbstractValidator<GetShipmentsQuery>
    {
        public GetShipmentsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}