using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class User
    {
        public User(Guid companyId, ContactDetailsVO contactDetailsVO, IEnumerable<AddressDC> addresses)
        {
            CompanyId = companyId;
            ContactDetails = contactDetailsVO;
        }

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