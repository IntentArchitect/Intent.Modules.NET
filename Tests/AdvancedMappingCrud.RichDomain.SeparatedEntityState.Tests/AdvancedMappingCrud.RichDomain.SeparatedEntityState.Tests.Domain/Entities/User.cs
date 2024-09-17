using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public class User : IHasDomainEvent
    {
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

        public Guid Id { get; set; }

        public Guid CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

        public ContactDetailsVO ContactDetails { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public Company TestEntity()
        {
            // [IntentFully]
            // TODO: Implement TestEntity (User) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }

        public AddressDC? TestDC(int index)
        {
            // [IntentFully]
            // TODO: Implement TestDC (User) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }

        public ContactDetailsVO TestVO()
        {
            // [IntentFully]
            // TODO: Implement TestVO (User) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }

        public void AddCollections(IEnumerable<AddressDC> addresses, IEnumerable<ContactDetailsVO> contacts)
        {
            // [IntentFully]
            // TODO: Implement AddCollections (User) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}