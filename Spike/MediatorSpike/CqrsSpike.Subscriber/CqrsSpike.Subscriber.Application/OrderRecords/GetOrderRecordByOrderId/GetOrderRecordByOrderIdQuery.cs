using Mediator;

namespace CqrsSpike.Subscriber.Application.OrderRecords.GetOrderRecordByOrderId
{
    public class GetOrderRecordByOrderIdQuery : IQuery<OrderRecordDto>
    {
        public GetOrderRecordByOrderIdQuery(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }
}