using EfCoreSoftDelete.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EfCoreSoftDelete.Domain.Entities
{
    public class Customer : ISoftDelete
    {
        public Customer()
        {
            Name = null!;
            PrimaryAddress = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public AddressBuilding? PrimaryBuilding { get; set; }

        public ICollection<Address> OtherAddresses { get; set; } = [];

        public Address PrimaryAddress { get; set; }

        void ISoftDelete.SetDeleted(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }
    }
}