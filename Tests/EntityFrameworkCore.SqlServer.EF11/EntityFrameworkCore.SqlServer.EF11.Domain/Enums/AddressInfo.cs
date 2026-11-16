using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Enums
{
    public class AddressInfo
    {
        public AddressInfo()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public AddressType AddressType { get; set; }

        public BuildingType? BuildingType { get; set; }
    }
}