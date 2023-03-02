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

    public class UserRoleDto : IMapFrom<Role>
    {
        public UserRoleDto()
        {
        }

        public static UserRoleDto Create(
            string name,
            Guid userId,
            Guid id)
        {
            return new UserRoleDto
            {
                Name = name,
                UserId = userId,
                Id = id,
            };
        }

        public string Name { get; set; }

        public Guid UserId { get; set; }

        public Guid Id { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Role, UserRoleDto>();
        }
    }
}