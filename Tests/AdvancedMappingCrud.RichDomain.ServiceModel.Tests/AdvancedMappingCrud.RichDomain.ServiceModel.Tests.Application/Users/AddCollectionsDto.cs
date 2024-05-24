using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    public class AddCollectionsDto
    {
        public AddCollectionsDto()
        {
            Addresses = null!;
            Contacts = null!;
        }

        public Guid Id { get; set; }
        public List<AddCollectionsAddAddressDCDto> Addresses { get; set; }
        public List<AddCollectionsAddContactDetailsVODto> Contacts { get; set; }

        public static AddCollectionsDto Create(
            Guid id,
            List<AddCollectionsAddAddressDCDto> addresses,
            List<AddCollectionsAddContactDetailsVODto> contacts)
        {
            return new AddCollectionsDto
            {
                Id = id,
                Addresses = addresses,
                Contacts = contacts
            };
        }
    }
}