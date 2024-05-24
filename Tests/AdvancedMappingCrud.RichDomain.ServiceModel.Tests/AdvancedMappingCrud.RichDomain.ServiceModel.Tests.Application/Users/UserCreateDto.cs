using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    public class UserCreateDto
    {
        public UserCreateDto()
        {
            ContactDetailsVO = null!;
            Addresses = null!;
        }

        public Guid CompanyId { get; set; }
        public UserCreateUserContactDetailsVODto ContactDetailsVO { get; set; }
        public List<UserCreateUserAddressDCDto> Addresses { get; set; }

        public static UserCreateDto Create(
            Guid companyId,
            UserCreateUserContactDetailsVODto contactDetailsVO,
            List<UserCreateUserAddressDCDto> addresses)
        {
            return new UserCreateDto
            {
                CompanyId = companyId,
                ContactDetailsVO = contactDetailsVO,
                Addresses = addresses
            };
        }
    }
}