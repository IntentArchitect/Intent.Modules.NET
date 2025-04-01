using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContacts
{
    public class CompanyContactDto : IMapFrom<CompanyContact>
    {
        public CompanyContactDto()
        {
        }

        public Guid Id { get; set; }
        public Guid ContactId { get; set; }

        public static CompanyContactDto Create(Guid id, Guid contactId)
        {
            return new CompanyContactDto
            {
                Id = id,
                ContactId = contactId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompanyContact, CompanyContactDto>();
        }
    }
}