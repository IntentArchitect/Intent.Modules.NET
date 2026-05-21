using AdvancedMapping.Repositories.Mapperly.Tests.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments
{
    public class CustomerSegmentsDto
    {
        public CustomerSegmentsDto()
        {
        }

        public Guid Id { get; set; }
        public Guid SegmentId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Confidence { get; set; }
        public SegmentType SegmentType { get; set; }
        public int SegmentPriority { get; set; }

        public static CustomerSegmentsDto Create(
            Guid id,
            Guid segmentId,
            Guid customerId,
            decimal confidence,
            SegmentType segmentType, int segmentPriority)
        {
            return new CustomerSegmentsDto
            {
                Id = id,
                SegmentId = segmentId,
                CustomerId = customerId,
                Confidence = confidence
,
                SegmentType = segmentType,
                SegmentPriority = segmentPriority
            };
        }
    }
}