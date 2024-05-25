using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Customers
{
    public class CreateCustomerUserDto
    {
        public CreateCustomerUserDto()
        {
            ContactDetailsVO = null!;
            Addresses = null!;
        }

        public Guid CompanyId { get; set; }
        public CreateCustomerUserContactDetailsVODto ContactDetailsVO { get; set; }
        public List<CreateCustomerUserAddressesDto> Addresses { get; set; }

        public static CreateCustomerUserDto Create(
            Guid companyId,
            CreateCustomerUserContactDetailsVODto contactDetailsVO,
            List<CreateCustomerUserAddressesDto> addresses)
        {
            return new CreateCustomerUserDto
            {
                CompanyId = companyId,
                ContactDetailsVO = contactDetailsVO,
                Addresses = addresses
            };
        }
    }
}