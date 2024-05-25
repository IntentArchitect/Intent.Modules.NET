using System;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    public class UserDto : IMapFrom<User>
    {
        public UserDto()
        {
        }

        public Guid CompanyId { get; set; }
        public Guid Id { get; set; }

        public static UserDto Create(Guid companyId, Guid id)
        {
            return new UserDto
            {
                CompanyId = companyId,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>();
        }
    }
}