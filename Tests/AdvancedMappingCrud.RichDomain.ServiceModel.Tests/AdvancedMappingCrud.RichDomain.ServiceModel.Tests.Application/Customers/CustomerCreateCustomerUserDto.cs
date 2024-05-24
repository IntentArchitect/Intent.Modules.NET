using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Customers
{
    public class CustomerCreateCustomerUserDto
    {
        public CustomerCreateCustomerUserDto()
        {
            ContactDetailsVO = null!;
            Addresses = null!;
        }

        public Guid CompanyId { get; set; }
        public CustomerCreateCustomerUserContactDetailsVODto ContactDetailsVO { get; set; }
        public List<CustomerCreateCustomerUserAddressesDto> Addresses { get; set; }

        public static CustomerCreateCustomerUserDto Create(
            Guid companyId,
            CustomerCreateCustomerUserContactDetailsVODto contactDetailsVO,
            List<CustomerCreateCustomerUserAddressesDto> addresses)
        {
            return new CustomerCreateCustomerUserDto
            {
                CompanyId = companyId,
                ContactDetailsVO = contactDetailsVO,
                Addresses = addresses
            };
        }
    }
}