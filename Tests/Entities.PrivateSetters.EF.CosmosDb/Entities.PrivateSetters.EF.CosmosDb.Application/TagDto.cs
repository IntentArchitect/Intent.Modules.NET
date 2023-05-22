using System;
using System.Collections.Generic;
using AutoMapper;
using Entities.PrivateSetters.EF.CosmosDb.Application.Common.Mappings;
using Entities.PrivateSetters.EF.CosmosDb.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.EF.CosmosDb.Application
{
    public class TagDto : IMapFrom<Tag>
    {
        public TagDto()
        {
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public static TagDto Create(Guid id, string name)
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