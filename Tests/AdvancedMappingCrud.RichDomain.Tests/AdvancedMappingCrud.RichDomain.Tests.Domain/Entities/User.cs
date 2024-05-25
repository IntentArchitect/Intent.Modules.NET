using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Entities
{
    public class User : IHasDomainEvent
    {
        private List<Address> _addresses = new List<Address>();
        public User(Guid companyId, ContactDetailsVO contactDetailsVO, IEnumerable<AddressDC> addresses)
        {
            CompanyId = companyId;
            ContactDetails = contactDetailsVO;
            Addresses = addresses.Select(a => new Address(a.Line1, a.Line2, a.City, a.Postal)).ToList();
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected User()
        {
            Company = null!;
            ContactDetails = null!;
        }

        public Guid Id { get; private set; }

        public Guid CompanyId { get; private set; }

        public virtual Company Company { get; private set; }

        public virtual IReadOnlyCollection<Address> Addresses
        {
            get => _addresses.AsReadOnly();
            private set => _addresses = new List<Address>(value);
        }

        public ContactDetailsVO ContactDetails { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public Company TestEntity()
        {
            return Company;
        }

        public AddressDC? TestDC(int index)
        {
            if (index < 0 || index >= Addresses.Count)
            {
                return null;
            }
            var address = Addresses.ElementAt(index);
            return new AddressDC(address.Line1, address.Line2, address.City, address.Postal);
        }

        public ContactDetailsVO TestVO()
        {
            return ContactDetails;
        }

        public void AddCollections(IEnumerable<AddressDC> addresses, IEnumerable<ContactDetailsVO> contacts)
        {
            // [IntentFully]
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}