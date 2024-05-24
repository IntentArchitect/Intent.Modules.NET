using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users.CreateUser
{
    public class CreateUserCommand : IRequest<Guid>, ICommand
    {
        public CreateUserCommand(Guid companyId,
            CreateUserContactDetailsVODto contactDetailsVO,
            List<CreateUserAddressDCDto> addresses)
        {
            CompanyId = companyId;
            ContactDetailsVO = contactDetailsVO;
            Addresses = addresses;
        }

        public Guid CompanyId { get; set; }
        public CreateUserContactDetailsVODto ContactDetailsVO { get; set; }
        public List<CreateUserAddressDCDto> Addresses { get; set; }
    }
}