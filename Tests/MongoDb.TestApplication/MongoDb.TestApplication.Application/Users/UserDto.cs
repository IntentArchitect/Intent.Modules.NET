using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Users
{

    public class UserDto : IMapFrom<User>
    {
        public UserDto()
        {
        }

        public static UserDto Create(
            long id,
            string username,
            List<UserRoleDto> roles)
        {
            return new UserDto
            {
                Id = id,
                Username = username,
                Roles = roles,
            };
        }

        public long Id { get; set; }

        public string Username { get; set; }

        public List<UserRoleDto> Roles { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>()
                .ForMember(d => d.Roles, opt => opt.MapFrom(src => src.Roles));
        }
    }
}