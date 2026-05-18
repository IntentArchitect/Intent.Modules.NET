using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities
{
    public class Container
    {
        public Container()
        {
            ContainerNumber = null!;
            SealNumber = null!;
        }

        public Guid Id { get; set; }

        public string ContainerNumber { get; set; }

        public string SealNumber { get; set; }

        public virtual ICollection<Vessel> Vessels { get; set; } = [];
    }
}