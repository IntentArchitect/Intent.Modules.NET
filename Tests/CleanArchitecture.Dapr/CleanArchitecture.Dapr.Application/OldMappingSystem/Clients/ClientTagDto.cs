using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.Dapr.Application.Common.Mappings;
using CleanArchitecture.Dapr.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Clients
{
    public class ClientTagDto : IMapFrom<Tag>
    {
        public ClientTagDto()
        {
            Name = null!;
            Id = null!;
        }

        public string Name { get; set; }
        public string Id { get; set; }

        public static ClientTagDto Create(string name, string id)
        {
            return new ClientTagDto
            {
                Name = name,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Tag, ClientTagDto>();
        }
    }
}