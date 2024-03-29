using System;
using System.Collections.Generic;
using AutoMapper;
using Finbuckle.SharedDatabase.TestApplication.Application.Common.Mappings;
using Finbuckle.SharedDatabase.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Application.Users
{

    public class UserDto : IMapFrom<User>
    {
        public UserDto()
        {
            Email = null!;
            Username = null!;
            Roles = null!;
        }

        public static UserDto Create(Guid id, string email, string username, List<UserRoleDto> roles)
        {
            return new UserDto
            {
                Id = id,
                Email = email,
                Username = username,
                Roles = roles
            };
        }

        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public List<UserRoleDto> Roles { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>()
                .ForMember(d => d.Roles, opt => opt.MapFrom(src => src.Roles));
        }
    }
}