using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using OutputCachingRedis.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Files
{
    public static class FilesDtoMappingExtensions
    {
        public static FilesDto MapToFilesDto(this Domain.Entities.Files projectFrom, IMapper mapper)
            => mapper.Map<FilesDto>(projectFrom);

        public static List<FilesDto> MapToFilesDtoList(this IEnumerable<Domain.Entities.Files> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToFilesDto(mapper)).ToList();
    }
}