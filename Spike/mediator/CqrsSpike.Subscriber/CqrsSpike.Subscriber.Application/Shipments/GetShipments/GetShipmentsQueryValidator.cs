using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.FluentValidation.QueryValidator", Version = "2.0")]

namespace CqrsSpike.Subscriber.Application.Shipments.GetShipments
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetShipmentsQueryValidator : AbstractValidator<GetShipmentsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetShipmentsQueryValidator()
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