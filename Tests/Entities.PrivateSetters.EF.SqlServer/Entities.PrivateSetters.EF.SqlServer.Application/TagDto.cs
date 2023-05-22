using System;
using System.Collections.Generic;
using AutoMapper;
using Entities.PrivateSetters.EF.SqlServer.Application.Common.Mappings;
using Entities.PrivateSetters.EF.SqlServer.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Application
{
    public class TagDto : IMapFrom<Tag>
    {
        public TagDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

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