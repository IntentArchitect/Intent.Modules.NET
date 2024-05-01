using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using OutputCachingRedis.Tests.Application.Common.Mappings;
using OutputCachingRedis.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Files
{
    public class FilesDto : IMapFrom<Domain.Entities.Files>
    {
        public FilesDto()
        {
            Content = null!;
        }

        public Guid Id { get; set; }
        public byte[] Content { get; set; }

        public static FilesDto Create(Guid id, byte[] content)
        {
            return new FilesDto
            {
                Id = id,
                Content = content
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.Files, FilesDto>();
        }
    }
}