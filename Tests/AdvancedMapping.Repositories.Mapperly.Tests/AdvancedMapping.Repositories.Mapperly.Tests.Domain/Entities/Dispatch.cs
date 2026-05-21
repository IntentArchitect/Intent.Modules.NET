using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities
{
    public class Dispatch
    {
        public Dispatch()
        {
            OriginLocation = null!;
            Document = null!;
        }

        public Guid Id { get; set; }

        public string OriginLocation { get; set; }

        public virtual DispatchDocument Document { get; set; }
    }
}