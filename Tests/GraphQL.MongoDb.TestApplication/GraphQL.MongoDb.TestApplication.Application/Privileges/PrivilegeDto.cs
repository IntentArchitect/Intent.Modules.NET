using System;
using System.Collections.Generic;
using AutoMapper;
using GraphQL.MongoDb.TestApplication.Application.Common.Mappings;
using GraphQL.MongoDb.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Privileges
{
    public class PrivilegeDto : IMapFrom<Privilege>
    {
        public PrivilegeDto()
        {
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public static PrivilegeDto Create(string id, string name, string? description)
        {
            return new PrivilegeDto
            {
                Id = id,
                Name = name,
                Description = description
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Privilege, PrivilegeDto>();
        }
    }
}