using System;
using System.Collections.Generic;
using AutoMapper;
using GraphQL.MongoDb.TestApplication.Application.Common.Mappings;
using GraphQL.MongoDb.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Users
{
    public class UserDto : IMapFrom<User>
    {
        public UserDto()
        {
        }

        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<UserAssignedPrivilegeDto> AssignedPrivileges { get; set; } = null!;

        public static UserDto Create(
            string id,
            string name,
            string surname,
            string email,
            List<UserAssignedPrivilegeDto> assignedPrivileges)
        {
            return new UserDto
            {
                Id = id,
                Name = name,
                Surname = surname,
                Email = email,
                AssignedPrivileges = assignedPrivileges
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>()
                .ForMember(d => d.AssignedPrivileges, opt => opt.MapFrom(src => src.AssignedPrivileges));
        }
    }
}