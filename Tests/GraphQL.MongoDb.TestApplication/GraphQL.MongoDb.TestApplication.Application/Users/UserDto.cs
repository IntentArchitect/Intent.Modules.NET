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
            Id = null!;
            Name = null!;
            Surname = null!;
            Email = null!;
            AssignedPrivileges = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public List<UserAssignedPrivilegeDto> AssignedPrivileges { get; set; }

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