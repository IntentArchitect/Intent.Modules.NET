using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.Folder;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Folder
{
    public static class FolderDtoMappingExtensions
    {
        public static FolderDto MapToFolderDto(this Domain.Entities.Folder.Folder projectFrom, IMapper mapper)
            => mapper.Map<FolderDto>(projectFrom);

        public static List<FolderDto> MapToFolderDtoList(this IEnumerable<Domain.Entities.Folder.Folder> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToFolderDto(mapper)).ToList();
    }
}