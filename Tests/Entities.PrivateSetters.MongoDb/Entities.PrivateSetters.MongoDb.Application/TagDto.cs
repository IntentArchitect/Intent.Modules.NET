using System;
using System.Collections.Generic;
using AutoMapper;
using Entities.PrivateSetters.MongoDb.Application.Common.Mappings;
using Entities.PrivateSetters.MongoDb.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Application
{
    public class TagDto : IMapFrom<Tag>
    {
        public TagDto()
        {
        }

        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;

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