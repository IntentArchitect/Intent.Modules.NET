using System;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users
{
    public class UserAddressDto : IMapFrom<Address>
    {
        public UserAddressDto()
        {
            Line1 = null!;
            Line2 = null!;
        }

        public Guid UserId { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public Guid Id { get; set; }

        public static UserAddressDto Create(Guid userId, string line1, string line2, Guid id)
        {
            return new UserAddressDto
            {
                UserId = userId,
                Line1 = line1,
                Line2 = line2,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Address, UserAddressDto>();
        }
    }
}