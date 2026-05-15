using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments
{
    public class CreateCustomerSegmentsDto
    {
        public CreateCustomerSegmentsDto()
        {
        }

        public Guid SegmentId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Confidence { get; set; }

        public static CreateCustomerSegmentsDto Create(Guid segmentId, Guid customerId, decimal confidence)
        {
            return new CreateCustomerSegmentsDto
            {
                SegmentId = segmentId,
                CustomerId = customerId,
                Confidence = confidence
            };
        }
    }
}