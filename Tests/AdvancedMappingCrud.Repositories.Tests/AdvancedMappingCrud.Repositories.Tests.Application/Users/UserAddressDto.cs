using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users
{
    public class UserAddressDto : IMapFrom<UserAddress>
    {
        public UserAddressDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            Postal = null!;
        }

        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Postal { get; set; }

        public static UserAddressDto Create(Guid userId, Guid id, string line1, string line2, string city, string postal)
        {
            return new UserAddressDto
            {
                UserId = userId,
                Id = id,
                Line1 = line1,
                Line2 = line2,
                City = city,
                Postal = postal
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UserAddress, UserAddressDto>();
        }
    }
}