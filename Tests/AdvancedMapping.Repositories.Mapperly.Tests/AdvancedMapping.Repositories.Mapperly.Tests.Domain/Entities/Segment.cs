using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities
{
    public class Segment
    {

        public Guid Id { get; set; }

        public SegmentType SegmentType { get; set; }

        public int Priority { get; set; }
    }
}