using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class OrderCustomerSegmentsDto
    {
        public OrderCustomerSegmentsDto()
        {
        }

        public Guid Id { get; set; }
        public Guid SegmentId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Confidence { get; set; }

        public static OrderCustomerSegmentsDto Create(Guid id, Guid segmentId, Guid customerId, decimal confidence)
        {
            return new OrderCustomerSegmentsDto
            {
                Id = id,
                SegmentId = segmentId,
                CustomerId = customerId,
                Confidence = confidence
            };
        }
    }
}