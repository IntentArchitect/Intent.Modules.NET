using System;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users
{
    public class UserDto : IMapFrom<User>
    {
        public UserDto()
        {
        }

        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }

        public static UserDto Create(Guid id, Guid companyId)
        {
            return new UserDto
            {
                Id = id,
                CompanyId = companyId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>();
        }
    }
}