using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.FluentValidation.QueryValidator", Version = "2.0")]

namespace CqrsSpike.Subscriber.Application.OrderRecords.GetOrderRecordByOrderId
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOrderRecordByOrderIdQueryValidator : AbstractValidator<GetOrderRecordByOrderIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderRecordByOrderIdQueryValidator()
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