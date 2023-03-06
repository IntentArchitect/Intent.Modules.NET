using System;
using System.Collections.Generic;
using AutoMapper;
using Finbuckle.SeparateDatabase.TestApplication.Application.Common.Mappings;
using Finbuckle.SeparateDatabase.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Finbuckle.SeparateDatabase.TestApplication.Application.Users
{

    public class UserRoleDto : IMapFrom<Role>
    {
        public UserRoleDto()
        {
        }

        public static UserRoleDto Create(
            Guid userId,
            string name,
            Guid id)
        {
            return new UserRoleDto
            {
                UserId = userId,
                Name = name,
                Id = id,
            };
        }

        public Guid UserId { get; set; }

        public string Name { get; set; }

        public Guid Id { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Role, UserRoleDto>();
        }
    }
}