using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.MultiKeyParents
{
    public static class MultiKeyParentDtoMappingExtensions
    {
        public static MultiKeyParentDto MapToMultiKeyParentDto(this MultiKeyParent projectFrom, IMapper mapper)
            => mapper.Map<MultiKeyParentDto>(projectFrom);

        public static List<MultiKeyParentDto> MapToMultiKeyParentDtoList(this IEnumerable<MultiKeyParent> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToMultiKeyParentDto(mapper)).ToList();
    }
}