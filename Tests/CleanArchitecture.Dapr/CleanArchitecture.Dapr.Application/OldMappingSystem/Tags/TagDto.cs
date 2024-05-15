using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.Dapr.Application.Common.Mappings;
using CleanArchitecture.Dapr.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Tags
{
    public class TagDto : IMapFrom<Tag>
    {
        public TagDto()
        {
            Id = null!;
            Name = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public static TagDto Create(string id, string name)
        {
            return new TagDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Tag, TagDto>();
        }
    }
}