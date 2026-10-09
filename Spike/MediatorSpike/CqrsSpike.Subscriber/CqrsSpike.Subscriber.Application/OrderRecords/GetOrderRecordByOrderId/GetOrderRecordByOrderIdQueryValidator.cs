using FluentValidation;

namespace CqrsSpike.Subscriber.Application.OrderRecords.GetOrderRecordByOrderId
{
    public class GetOrderRecordByOrderIdQueryValidator : AbstractValidator<GetOrderRecordByOrderIdQuery>
    {
        public GetOrderRecordByOrderIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}