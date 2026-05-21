using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Users
{
    public class UserDto : IMapFrom<User>
    {
        public UserDto()
        {
            Addresses = null!;
        }

        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public List<UserUserAddressDto> Addresses { get; set; }

        public static UserDto Create(Guid id, Guid companyId, List<UserUserAddressDto> addresses)
        {
            return new UserDto
            {
                Id = id,
                CompanyId = companyId,
                Addresses = addresses
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>()
                .ForMember(d => d.Addresses, opt => opt.MapFrom(src => src.Addresses));
        }
    }
}