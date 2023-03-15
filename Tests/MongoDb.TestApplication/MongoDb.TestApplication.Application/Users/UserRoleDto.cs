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

    public class UserRoleDto : IMapFrom<Role>
    {
        public UserRoleDto()
        {
        }

        public static UserRoleDto Create(
            string name,
            Guid id)
        {
            return new UserRoleDto
            {
                Name = name,
                Id = id,
            };
        }

        public string Name { get; set; }

        public Guid Id { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Role, UserRoleDto>();
        }
    }
}