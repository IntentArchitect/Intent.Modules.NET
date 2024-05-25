using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users.AddCollectionsUser
{
    public class AddCollectionsUserCommand : IRequest, ICommand
    {
        public AddCollectionsUserCommand(Guid id, List<AddAddressDCDto> addresses, List<AddContactDetailsVODto> contacts)
        {
            Id = id;
            Addresses = addresses;
            Contacts = contacts;
        }

        public Guid Id { get; set; }
        public List<AddAddressDCDto> Addresses { get; set; }
        public List<AddContactDetailsVODto> Contacts { get; set; }
    }
}