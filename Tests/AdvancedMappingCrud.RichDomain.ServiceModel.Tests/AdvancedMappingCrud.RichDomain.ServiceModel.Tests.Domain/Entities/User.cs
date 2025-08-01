using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities
{
    public class User : IHasDomainEvent
    {
        private List<Address> _addresses = [];

        public User(Guid companyId, ContactDetailsVO contactDetailsVO, IEnumerable<AddressDC> addresses)
        {
            CompanyId = companyId;
            ContactDetails = contactDetailsVO;
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

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public Company TestEntity()
        {
            // TODO: Implement TestEntity (User) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }

        public AddressDC? TestDC(int index)
        {
            // TODO: Implement TestDC (User) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }

        public ContactDetailsVO TestVO()
        {
            // TODO: Implement TestVO (User) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }

        public void AddCollections(IEnumerable<AddressDC> addresses, IEnumerable<ContactDetailsVO> contacts)
        {
            // TODO: Implement AddCollections (User) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}