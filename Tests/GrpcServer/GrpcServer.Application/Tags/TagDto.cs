using System;
using AutoMapper;
using GrpcServer.Application.Common.Mappings;
using GrpcServer.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.Tags
{
    public class TagDto : IMapFrom<Tag>
    {
        public TagDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }

        public static TagDto Create(string name, Guid id)
        {
            return new TagDto
            {
                Name = name,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Tag, TagDto>();
        }
    }
}