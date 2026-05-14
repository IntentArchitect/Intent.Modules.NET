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
        public ClassificationSource ClassificationSource { get; set; }
        public decimal Confidence { get; set; }

        public static CustomerSegmentsDto Create(
            Guid id,
            Guid segmentId,
            Guid customerId,
            ClassificationSource classificationSource,
            decimal confidence)
        {
            return new CustomerSegmentsDto
            {
                Id = id,
                SegmentId = segmentId,
                CustomerId = customerId,
                ClassificationSource = classificationSource,
                Confidence = confidence
            };
        }
    }
}